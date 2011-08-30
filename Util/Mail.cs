/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/

#endregion Header

namespace WebGrid.Util
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Net.Mail;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;

    using WebGrid.Collections;
    using WebGrid.Config;
    using WebGrid.Data;
    using WebGrid.Enums;

    /// <summary>
    /// Contains various methods to validate and send e-mail.
    /// Asynchronous service "SendMail" are logged at Windows EventLog (Application category) if any error. 
    /// </summary>
    public class Mail
    {
        #region Fields

        private SmtpClient mailSender;

        #endregion Fields

        #region Delegates

        private delegate void SendMailMessageDelegate(MailMessage message, string smtpserver);

        #endregion Delegates

        #region Methods

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="emailAddresses">A string array of email addresses to send.</param>
        /// <param name="senderEmailAddress">The sender's email address</param>
        /// <param name="grid">The grid.</param>
        /// <returns>Number of email's sent</returns>
        /// <remarks>This asynchronous method are logged at Windows EventLog (Application category) if any error. </remarks>
        public static int SendEmail(string[] emailAddresses, string senderEmailAddress, Grid grid)
        {
            StringBuilder sb = new StringBuilder(string.Empty);

            sb.Append("<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\">");

            // Render header for rows if more then 1 row.
            if (grid.Rows.Count > 1)
            {
                sb.Append("<tr>");
                foreach (Column column in grid.Columns)
                {
                    if (column.Visibility == Visibility.Both || column.Visibility == Visibility.Grid)
                        sb.AppendFormat("<td>{0}</td>", column.Title);
                }
                sb.Append("</tr>");
            }
            Mail wgmail = new Mail();
            ArrayList attachments = new ArrayList();

            // Render column foreach row if one row, else render all columns in one row.
            foreach (Row gridrow in grid.Rows)
            {
                if (gridrow == null)
                    continue;
                if (grid.Rows.Count > 1)
                    sb.Append("<tr>");
                foreach (Column column in gridrow.Columns)
                {
                    if (column.Visibility == Visibility.None || column.ColumnType == ColumnType.Chart ||
                        column.ColumnType == ColumnType.SystemColumn)
                        continue;

                    if (column.ColumnType == ColumnType.File && grid.Rows.Count == 1)
                    {
                        File attachcolumn = (File) column;

                        Attachment attachfile =
                            new Attachment(attachcolumn.AbsoluteDirectoryFileName(gridrow[column.ColumnId]).Replace("NULL", grid.InternalId),
                                           "UUEncode");

                        attachments.Add(attachfile);
                        continue;
                    }
                    object value = gridrow[column.ColumnId].Value;
                    if (column.ColumnType == ColumnType.ColumnTemplate)
                    {
                        if (grid.DisplayView == DisplayView.Detail && ((ColumnTemplate)column).DetailViewTemplate != null)
                            value = ((ColumnTemplate)column).RenderTemplate(gridrow[column.ColumnId]);
                        else if (grid.DisplayView == DisplayView.Grid && ((ColumnTemplate)column).GridViewTemplate != null)
                            value = ((ColumnTemplate)column).RenderTemplate(gridrow[column.ColumnId]);
                    }
                    switch (column.ColumnType)
                    {
                        case ColumnType.Foreignkey:
                            {
                                RowCollection row = ((Foreignkey) column).Table.Rows;

                                for (int i = 0; i < row.Count; i++)
                                {
                                    if (row[i][column.ColumnId].Value == null ||
                                        row[i].PrimaryKeyUpdateValues != row[i][column.ColumnId].Value.ToString())
                                        continue;
                                    value =
                                        Foreignkey.BuildDisplayText(i,
                                                                              ((Foreignkey) column).ValueColumn,
                                                                              ((Foreignkey) column).Table);
                                    break;
                                }
                            }
                            break;
                        case ColumnType.ManyToMany:
                            {
                                value = string.Empty;
                                ManyToManyCollection items = ((ManyToMany) column).LoadItems(gridrow[column.ColumnId]);

                                for (int i = 0; i < items.Count; i++)
                                {
                                    if (items[i].Checked)
                                        if (value == null)
                                            value = items[i].DisplayText;
                                        else
                                            value += string.Format(" , {0}", items[i].DisplayText);
                                }
                            }
                            break;
                    }
                    if ((column.Visibility == Visibility.Both || column.Visibility == Visibility.Grid)
                        && grid.Rows.Count > 1)
                        sb.AppendFormat("<td>{0}</td>", value);
                    else if ((column.Visibility == Visibility.Both || column.Visibility == Visibility.Detail)
                             && grid.Rows.Count == 1)
                        if (!column.HideDetailTitle)
                            sb.AppendFormat("<tr><td><b>{0}</b></td></tr><tr><td>{1}</td></tr>", column.Title, value);
                        else
                            sb.AppendFormat("<tr><td></td></tr><tr><td>{0}</td></tr>", value);

                }

                sb.Append("</tr>");
            }
            sb.Append("</table>");

            string smtpserver = GridConfig.Get("WGSMTPSERVER", (string) null);

            if (grid.MailForm.SmtpServer != null)
                smtpserver = grid.MailForm.SmtpServer;
            string body = sb.ToString();
            int sentemails = 0;
            foreach (string emailaddress in emailAddresses)
            {
                if (!ValidEmail(emailaddress)) continue;
                sentemails++;
                MailMessage message = new MailMessage(new MailAddress(emailaddress), new MailAddress(emailaddress));

                for (int i = 0; i < attachments.Count; i++)
                    message.Attachments.Add((Attachment) attachments[i]);

                message.IsBodyHtml = true;

                message.From = string.IsNullOrEmpty(senderEmailAddress) == false
                                   ? new MailAddress(senderEmailAddress)
                                   : new MailAddress(emailaddress);
                message.Subject = grid.MailForm.Subject ?? grid.Title;
                message.Body = HttpUtility.HtmlDecode(body);
                wgmail.SendEmail(message, smtpserver);
            }
            return sentemails;
        }

        ///<summary>
        /// Send a lists of mails.
        ///</summary>
        ///<param name="mails">List of mails</param>
        ///<param name="smtphost">smtphost to use</param>
        public static void SendMailBulk(List<MailMessage> mails, string  smtphost)
        {
            foreach (MailMessage mail in mails)
            {
                MailMessage message = mail;
                ThreadPool.QueueUserWorkItem(
                    delegate
                        {
                            try
                            {
                                SmtpClient tmp = new SmtpClient();
                                tmp.Host = smtphost;
                                tmp.Send(message);
                            }
                            catch (Exception ex)
                            {
                                using (EventLog Logger = new EventLog())
                                {
                                    Logger.Source = "WebGrid.Util.Mail.SendMailBulk";
                                    Logger.WriteEntry(ex.Message, EventLogEntryType.Error);
                                }

                            }
                        }
                    );
            }
        }

        /// <summary>
        /// Checks whether an array of email addresses are valid.
        /// </summary>
        /// <param name="emailAddresses">A string array of email addresses to check.</param>
        /// <returns>True if the addresses is valid. False if one or more addresses is bogus.</returns>
        /// <remarks>This asynchronous method are logged at Windows EventLog (Application category) if any error. </remarks>
        public static bool ValidEmail(string[] emailAddresses)
        {
            if (emailAddresses == null)
                return false;

            for (int i = 0; i < emailAddresses.Length; i++)
                if (ValidEmail(emailAddresses[i]) == false)
                    return false;
            return true;
        }

        /// <summary>
        /// Checks whether an email address is valid.
        /// </summary>
        /// <param name="emailAddress">The email address to check.</param>
        /// <returns>True if the address is valid. False if the address is bogus.</returns>
        public static bool ValidEmail(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return false;

            Regex validateEmail = new Regex("^[\\w\\.\\-_]+@([\\w\\.\\-_]+\\.)+[A-Za-z]{2,4}$", RegexOptions.IgnoreCase);
            return validateEmail.Match(emailAddress).Success;
        }

        /// <summary>
        /// Checks whether a comma separated list of email addresses are valid.
        /// </summary>
        /// <param name="commaSeparatedEmailAddresses">A comma separated list of email addresses to check.</param>
        /// <returns>True if the addresses is valid. False if one or more addresses is bogus.</returns>
        public static bool ValidEmails(string commaSeparatedEmailAddresses)
        {
            if (commaSeparatedEmailAddresses == null)
                return false;

            string email = commaSeparatedEmailAddresses.Replace(" ", string.Empty).Replace(";", ",");
            return ValidEmail(email.Split(","[0]));
        }

        /// <summary>
        /// This method is for sending out html emails asynchronously, the
        /// e-mail addresses are added into a thread-pool by the program and processed.
        /// Useful if your application need to send mass e-mails.
        /// IMPORTANT: "Async" Property for your web page has to be turned on to use this Method.
        /// </summary>
        /// <param name="message">E-mail message you want to send</param>
        /// <param name="smtpserver">smtp server for this e-mail</param>
        /// <remarks>This asynchronous method are logged at Windows EventLog (Application category) if any error. </remarks>
        public void SendEmail(MailMessage message, string smtpserver)
        {
            if (string.IsNullOrEmpty(smtpserver))
                smtpserver = GridConfig.Get("WGSMTPSERVER", (string)null);

            SendMailMessageDelegate dc = SendMailMessage;

            Asynchronous.FireAndForget(dc, message, smtpserver);
        }

        /// <summary>
        /// This method is for sending out html emails asynchronously, the
        /// e-mail addresses are added into a thread-pool by the program and processed.
        /// Useful if your application need to send mass e-mails.
        /// IMPORTANT: "Async" Property for your web page has to be turned on to use this Method.
        /// </summary>
        /// <param name="toemailaddress">The email address to be added and sent asynchronously.</param>
        /// <param name="fromemailaddress">The sender's email address.</param>
        /// <param name="subject">Subject for this e-mail</param>
        /// <param name="body">Content of this e-mail</param>
        /// <param name="mailEncoding">Encoding of this e-mail</param>
        /// <param name="mailpriority">Priority of this e-mail</param>
        /// <param name="smtpserver">smtp server for this e-mail</param>
        /// <remarks>This asynchronous method are logged at Windows EventLog (Application category) if any error. </remarks>
        public void SendEmail(string toemailaddress, string fromemailaddress, string subject, string body,
            Encoding mailEncoding,
            MailPriority mailpriority, string smtpserver)
        {
            MailMessage Message = new MailMessage(new MailAddress(fromemailaddress), new MailAddress(toemailaddress));

            Message.Subject = subject;
            Message.Body = body;
            Message.BodyEncoding = mailEncoding;
            Message.Priority = mailpriority;

            SendEmail(Message, smtpserver);
        }

        private static void mailSender_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null) return;
            EventLog Logger = new EventLog();

            Logger.Source = "WebGrid.Util.Mail";
            Logger.WriteEntry(e.Error.ToString(), EventLogEntryType.Error);
            Logger.Dispose();
        }

        private void SendMailMessage(MailMessage message, string smtpserver)
        {
            try
            {
                mailSender = new SmtpClient();

                if (string.IsNullOrEmpty(mailSender.Host))
                    mailSender.Host = smtpserver;
                mailSender.Send(message);
                mailSender.SendCompleted += mailSender_SendCompleted;
            }
            catch (Exception ee)
            {
                EventLog Logger = new EventLog();

                Logger.Source = "WebGrid.Util.Mail";
                Logger.WriteEntry(ee.ToString(), EventLogEntryType.Error);
                Logger.Dispose();
            }
        }

        #endregion Methods
    }
}