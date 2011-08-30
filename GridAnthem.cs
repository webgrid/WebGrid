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

namespace WebGrid
{
    using System;
    using System.ComponentModel;

    using WebGrid.Config;

    public partial class Grid
    {
        #region Fields

        private bool _updateAfterCallBack = true;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the control should convert child control post backs into callbacks.
        /// </summary>
        /// <value>
        /// 	<strong>true</strong> if the control should convert child control post backs to
        /// callbacks; otherwise, <strong>false</strong>. The default is
        /// <strong>true</strong>.
        /// </value>
        /// <remarks>
        /// Only controls that <see cref="Anthem.Manager"/> recognizes will be converted.
        /// </remarks>
        [Category("Anthem Ajax")]
        [DefaultValue(true)]
        [Description("True if this control should convert child control post backs into callbacks.")]
        public virtual bool AddCallBacks
        {
            get
            {
                object obj = ViewState["AddCallBacks"];
                return obj == null || (bool)obj;
            }
            set { ViewState["AddCallBacks"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control should be updated after each callback.
        /// Also see <see cref="UpdateAfterCallBack"/>.
        /// </summary>
        /// <value>
        /// 	<strong>true</strong> if the the control should be updated; otherwise,
        /// <strong>false</strong>. The default is <strong>true</strong>.
        /// </value>
        [Category("Anthem Ajax")]
        [DefaultValue(true)]
        [Description("True if this control should be updated after each callback.")]
        public virtual bool AutoUpdateAfterCallBack
        {
            get
            {
                return true;
                /* object obj = this.ViewState["AutoUpdateAfterCallBack"];
                if (obj == null)
                    return true;
                else
                    return (bool)obj;*/
            }
            set
            {
                if (value) UpdateAfterCallBack = true;
                ViewState["AutoUpdateAfterCallBack"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the javascript function to execute on the client if the callback is
        /// cancelled. See <see cref="PreCallBackFunction"/>.
        /// </summary>
        [Category("Anthem Ajax")]
        [DefaultValue("")]
        [Description("The javascript function to call on the client if the callback is cancelled.")]
        public virtual string CallBackCancelledFunction
        {
            get
            {
                string text = (string)ViewState["CallBackCancelledFunction"];
                return text ?? string.Empty;
            }
            set { ViewState["CallBackCancelledFunction"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control uses callbacks instead of post backs to post data to the server.
        /// </summary>
        /// <value>
        /// 	<strong>true</strong> if the the control uses callbacks; otherwise,
        /// <strong>false</strong>. The default is <strong>true</strong>.
        /// </value>
        [Category("Anthem Ajax")]
        [DefaultValue(true)]
        [Description("True if this control uses callbacks instead of post backs to post data to the server.")]
        public virtual bool EnableCallBack
        {
            get
            {
                object obj = ViewState["EnableCallBack"];
                return obj == null ? GridConfig.Get("WGEnableCallBack", true) : (bool)obj;
            }
            set { ViewState["EnableCallBack"] = value; }
        }

        /// <summary>
        /// Sets or gets if to use WebGrid's  alert window for errors during callbacks.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable call back error alert]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Only available when 'EnableCallBack' is enabled.
        /// </remarks>
        [Category("Anthem Ajax")]
        [DefaultValue(true)]
        [Description(
            "Sets or gets if to use WebGrid's  alert window for errors during callbacks. (Only available when 'EnableCallBack' is enabled)"
            )]
        public virtual bool EnableCallBackErrorAlert
        {
            get
            {
                object obj = ViewState["EnableCallBackErrorAlert"];
                return obj == null ? GridConfig.Get("EnableCallBackErrorAlert", true) : (bool)obj;
            }
            set { ViewState["EnableCallBackErrorAlert"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is enabled on the client during callbacks.
        /// </summary>
        /// <value>
        /// 	<strong>true</strong> if the the control is enabled; otherwise,
        /// <strong>false</strong>. The default is <strong>false</strong>.
        /// </value>
        /// <remarks>Not all HTML elements support this property.</remarks>
        [Category("Anthem Ajax")]
        [DefaultValue(false)]
        [Description("True if this control is enabled on the client during callbacks.")]
        public virtual bool EnabledDuringCallBack
        {
            get
            {
                object obj = ViewState["EnabledDuringCallBack"];
                return obj != null && (bool)obj;
            }
            set { ViewState["EnabledDuringCallBack"] = value; }
        }

        /// <summary>
        /// Gets or sets the javascript function to execute on the client after the callback
        /// response is received.
        /// </summary>
        /// <remarks>
        /// The callback response is passed into the PostCallBackFunction as the one and only
        /// parameter.
        /// </remarks>
        /// <example>
        /// 	<code lang="JScript" description="This example shows a PostCallBackFunction that displays the error if there is one.">
        /// function AfterCallBack(result) {
        ///   if (result.error != null &amp;&amp; result.error.length &gt; 0) {
        ///     alert(result.error);
        ///   }
        /// }
        ///     </code>
        /// </example>
        [Category("Anthem Ajax")]
        [DefaultValue("")]
        [Description("The javascript function to call on the client after the callback response is received.")]
        public virtual string PostCallBackFunction
        {
            get
            {
                string text = (string)ViewState["PostCallBackFunction"];

                return text ?? string.Empty;
            }
            set { ViewState["PostCallBackFunction"] = value; }
        }

        /// <summary>
        /// Gets or sets the javascript function to execute on the client before the callback
        /// is made.
        /// </summary>
        /// <remarks>The function should return false on the client to cancel the callback.</remarks>
        [Category("Anthem Ajax")]
        [DefaultValue("")]
        [Description("The javascript function to call on the client before the callback is made.")]
        public virtual string PreCallBackFunction
        {
            get
            {
                string text = (string) ViewState["PreCallBackFunction"];
                if (string.IsNullOrEmpty(text))
                    text += "wgcleanclientobjects()";
                else if (!text.Contains("wgcleanclientobjects"))
                    text += "wgcleanclientobjects()";

                return text;
            }
            set { ViewState["PreCallBackFunction"] = value; }
        }

        /// <summary>Gets or sets the text to display on the client during the callback.</summary>
        /// <remarks>
        /// If the HTML element that invoked the callback has a text value (such as &lt;input
        /// type="button" value="Run"&gt;) then the text of the element is updated during the
        /// callback, otherwise the associated &lt;label&gt; text is updated during the callback.
        /// If the element does not have a text value, and if there is no associated &lt;label&gt;,
        /// then this property is ignored.
        /// </remarks>
        [Category("Anthem Ajax")]
        [DefaultValue("")]
        [Description("The text to display during the callback.")]
        public virtual string TextDuringCallBack
        {
            get
            {
                string text = (string)ViewState["TextDuringCallBack"];
                return text ?? string.Empty;
            }
            set { ViewState["TextDuringCallBack"] = value; }
        }

        /// <summary>
        /// Gets or sets a value which indicates whether the control should be updated after the current callback.
        /// Also see <see cref="AutoUpdateAfterCallBack"/>.
        /// </summary>
        /// <value>
        /// 	<strong>true</strong> if the the control should be updated; otherwise,
        /// <strong>false</strong>. The default is <strong>true</strong>.
        /// </value>
        [Browsable(false)]
        [DefaultValue(true)]
        public virtual bool UpdateAfterCallBack
        {
            get { return _updateAfterCallBack; }
            set { _updateAfterCallBack = value; }
        }

        /// <summary>
        /// Overrides the Visible property so that Anthem.Manager can track the visibility.
        /// </summary>
        /// <value>
        /// 	<strong>true</strong> if the control is rendered on the client; otherwise
        /// <strong>false</strong>. The default is <strong>true</strong>.
        /// </value>
        public override bool Visible
        {
            get { return EnableCallBack ? Anthem.Manager.GetControlVisible(this, ViewState, DesignMode) : base.Visible; }
            set
            {
                if (EnableCallBack)
                    Anthem.Manager.SetControlVisible(ViewState, value);
                else
                    base.Visible = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeEnableCallBack()
        {
            return EnableCallBack != GridConfig.Get("WGEnableCallBack", true);
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializePreCallBackFunction()
        {
            return !string.IsNullOrEmpty(PreCallBackFunction.Trim()) && !"wgcleanclientobjects()".Equals(PreCallBackFunction);
        }

        #endregion Methods
    }
}