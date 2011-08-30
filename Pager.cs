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
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Enums;
    using Util;

    /// <summary>
    /// Control that renders navigation controls.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [Description("Grid pager")]
    [MergableProperty(false)]
    [Browsable(false)]
    public  class Pager : IComponent
    {
        #region Fields

        //private new Control Parent;
        internal Grid m_PagerGrid;

        private const string gridslider = @"$(document).ready(function() {{
                                                         $(""#{3}slider"").slider({{
                                                             orientation: ""{7}"",
                                                             value: {0},
                                                             range: {5},
                                                             min: 0,
                                                             step: {4},
                                                             max: {2},
                                                             animate: true ,
                                                              change: function(event, ui) {{ {6}.refresh(); }},

                                                             slide: function(event, ui) {{
                                                             $(""#{3}slidervalue"").val('' + ui.value);
                                                             }}
                                                         }});
                                                         $(""#{3}slidervalue"").val('' + $(""#{3}slider"").slider(""value""));
                                                     }});";
        private const string gridsliderrange = @"$(document).ready(function() {{
                                                         $(""#{3}slider"").slider({{
                                                             orientation: ""{7}"",
                                                             range: {5},
                                                             min: 0,

                                                             step: {4},
                                                             max: {2},
                                                             animate: true ,
                                                             values: [{0}, {1}],
                                                              change: function(event, ui) {{ {6}.refresh(); }},

                                                             slide: function(event, ui) {{
                                                             $(""#{3}slidervalue"").val('' + ui.values[0] + ' - '+ ui.values[1]);
                                                             }}
                                                         }});
                                                         $(""#{3}slidervalue"").val('' + $(""#{3}slider"").slider(""values"", 0) + ' - ' + $(""#{3}slider"").slider(""values"", 1));
                                                     }});";

        private string _sliderValue;
        private int m_CurrentPageDigits;
        private bool m_EventRanpreRender;
        private string m_ImagePath = "/images/";
        private NavigateStyle m_NavigateStyle = NavigateStyle.Center;
        private string m_PageBarTemplate = " {0} ",
                       m_PageDropDownTemplate = "{0} / {1}",
                       m_PreviousTemplate = "<img border=\"0\" src=\"{0}previous.gif\" alt=\"{1}\"/>",
                       m_NextTemplate = "<img border=\"0\" src=\"{0}next.gif\" alt=\"{1}\" />",
                       m_FirstTemplate = "<img border=\"0\" src=\"{0}first.gif\" alt=\"{1}\" />",
                       m_LastTemplate = "<img border=\"0\" src=\"{0}last.gif\" alt=\"{1}\" />";
        private PagerType m_PagerType = PagerType.Standard;
        private int m_PagerWidth = 10, m_Pages;
        private string m_SelectedPageBarTemplate = " {0} ";
        int[] m_slidervalues;
        string pagercontent;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Pager"/> class.
        /// </summary>
        public Pager()
        {
            SliderWidth = Unit.Pixel(500);
            SliderHeight = Unit.Pixel(500);
            m_PagerGrid = null;
            SliderStep = 1;
        }

        /// <summary>
        /// Creates a new Pager control.
        /// </summary>
        /// <param name="grid">The parent object in which to render the control.</param>
        public Pager(Grid grid)
        {
            m_PagerGrid = grid;
            SliderStep = 1;
            SliderWidth = Unit.Pixel(500);
            SliderHeight = Unit.Pixel(500);
            ImagePath = grid.ImagePath;
            InitProperties(grid);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Adds an event handler to listen to the Disposed event on the component.
        /// </summary>
        public event EventHandler Disposed;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the number of digits to display. -1 means auto.
        /// </summary>
        /// <value>The number of digits to display.</value>
        [DefaultValue(-1)]
        public int CurrentPageDigits
        {
            get { return m_CurrentPageDigits == -1 ? m_Pages.ToString().Length : m_CurrentPageDigits; }
            set { m_CurrentPageDigits = value; }
        }

        /// <summary>
        /// Should the show 'all' button be visible ?
        /// </summary>
        /// <value><c>true</c> if [display all]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// It is recommended to disable this property if your application can display huge amounts of database records.
        /// </remarks>
        [DefaultValue(true)]
        public bool DisplayAll
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets whether "first" and "last" button should be/are displayed in the pager.
        /// </summary>
        /// <value><c>true</c> if [display first last]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool DisplayFirstLast
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets whether "next" and "previous" button should be/are displayed in the pager.
        /// </summary>
        /// <value><c>true</c> if [display previous next]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool DisplayPreviousNext
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the template for the first button.
        /// </summary>
        /// <value>The first HTML template.</value>
        [DefaultValue("<img border=\"0\" src=\"{0}first.gif\" alt=\"{1}\" />")]
        public string FirstTemplate
        {
            get { return m_FirstTemplate; }
            set { m_FirstTemplate = value; }
        }

        /// <summary>
        /// Gets or sets the slider values.
        /// </summary>
        public int[] GetSliderValues
        {
            get
            {
                if (m_PagerGrid.PagerSettings.SliderValue == null) return null;
                if (m_slidervalues != null)
                   return m_slidervalues;
                string[] values = m_PagerGrid.PagerSettings.SliderValue.Split('-');

                switch (values.Length)
                {
                    //Ranged slider
                    case 2:
                        {
                            m_slidervalues = new int[2];
                            int startrecord, endrecord;
                            if (int.TryParse(values[0].Trim(), out startrecord) && int.TryParse(values[1].Trim(), out endrecord))
                            {
                                m_slidervalues[0] = startrecord;
                                m_slidervalues[1] = endrecord;
                            }
                            break;
                        }
                    case 1:
                        {
                            m_slidervalues = new int[1];
                            int startrecord;
                            if (int.TryParse(values[0].Trim(), out startrecord))
                            {
                                if (startrecord == 0)
                                    startrecord = 1;
                                m_slidervalues[0] = startrecord;
                            }
                            break;
                        }
                }
                return m_slidervalues;
            }
            set
            {
                m_slidervalues = value;
            }
        }

        /// <summary>
        /// Gets or sets the path where the images for grid pager is located on disk.
        /// </summary>
        /// <value>The image path.</value>
        public string ImagePath
        {
            get { return m_ImagePath; }
            set { m_ImagePath = value; }
        }

        /// <summary>
        /// Gets or sets the template for the last button.
        /// </summary>
        /// <value>The last HTML template.</value>
        [DefaultValue("<img border=\"0\" src=\"{0}last.gif\" alt=\"{1}\" />")]
        public string LastTemplate
        {
            get { return m_LastTemplate; }
            set { m_LastTemplate = value; }
        }

        /// <summary>
        /// Gets or sets the type of style used for navigating.
        /// </summary>
        /// <value>The navigation style.</value>
        [DefaultValue(NavigateStyle.Center)]
        public NavigateStyle NavigateStyle
        {
            get { return m_NavigateStyle; }
            set { m_NavigateStyle = value; }
        }

        /// <summary>
        /// Gets or sets the template for the next button.
        /// </summary>
        /// <value>The next HTML template.</value>
        [DefaultValue("<img border=\"0\" src=\"{0}next.gif\" alt=\"{1}\" />")]
        public string NextTemplate
        {
            get { return m_NextTemplate; }
            set { m_NextTemplate = value; }
        }

        /// <summary>
        /// Gets or sets a template to be used for the pager.
        /// </summary>
        /// <value>The page bar HTML template.</value>
        [DefaultValue(" {0} ")]
        public string PageBarTemplate
        {
            get { return m_PageBarTemplate; }
            set { m_PageBarTemplate = value; }
        }

        /// <summary>
        /// Gets or sets the template for the dropdown pager.
        /// </summary>
        /// <value>The page drop down template.</value>
        [DefaultValue("{0} / {1}")]
        public string PageDropDownTemplate
        {
            get { return m_PageDropDownTemplate; }
            set { m_PageDropDownTemplate = value; }
        }

        /// <summary>
        /// Gets or sets the type of navigation pager that should be displayed.
        /// </summary>
        /// <value>The type of the pager.</value>
        [DefaultValue(PagerType.Standard)]
        public PagerType PagerType
        {
            get { return m_PagerType; }
            set { m_PagerType = value; }
        }

        /// <summary>
        /// The number of pages to show at once.
        /// </summary>
        /// <value>The width of the pager.</value>
        [DefaultValue(3)]
        public int PagerWidth
        {
            get { return m_PagerWidth; }
            set { m_PagerWidth = value; }
        }

        /// <summary>
        /// Gets or sets the template for the previous button.
        /// </summary>
        /// <value>The previous HTML template.</value>
        [DefaultValue("<img border=\"0\" src=\"{0}previous.gif\" alt=\"{1}\"/>")]
        public string PreviousTemplate
        {
            get { return m_PreviousTemplate; }
            set { m_PreviousTemplate = value; }
        }

        /// <summary>
        /// Gets or sets the template for the current page.
        /// </summary>
        /// <value>The selected page bar template.</value>
        [DefaultValue(" {0} ")]
        public string SelectedPageBarTemplate
        {
            get { return m_SelectedPageBarTemplate; }
            set { m_SelectedPageBarTemplate = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.ComponentModel.ISite"></see> associated with the <see cref="T:System.ComponentModel.IComponent"></see>.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.ISite"></see> object associated with the component; or null, if the component does not have a site.</returns>
        [Browsable(false)]
        public ISite Site
        {
            get { return m_PagerGrid.Site; }
            set { m_PagerGrid.Site = value; }
        }

        /// <summary>
        /// Sets or gets the slider width used inside this grid. Default size is 500px.
        /// </summary>
        [DefaultValue(500)]
        public Unit SliderHeight
        {
            get; set;
        }

        /// <summary>
        /// Determines the size or amount of each interval or step the slider takes between min and max. 
        /// The full specified value range of the slider (max - min) needs to be evenly divisible by the step.
        /// Default is grid.PageSize
        /// </summary>
        [DefaultValue(1)]
        public int SliderStep
        {
            get; set;
        }

        /// <summary>
        /// Determines the value of the slider, if there's more then only one handle then the values are seperated by ';'
        /// </summary>
        [DefaultValue(null)]
        public string SliderValue
        {
            get
            {

                if (m_PagerGrid != null && Grid.GotHttpContext && m_PagerGrid.GetState("SliderValue") != null)
                    _sliderValue = m_PagerGrid.GetState("SliderValue") as string;

                return _sliderValue;

            }
            internal set
            {
                if (m_PagerGrid != null && Grid.GotHttpContext)
                    m_PagerGrid.SetState("SliderValue", value);

                _sliderValue = value;
            }
        }

        /// <summary>
        /// Sets or gets the slider width used inside this grid. Default size is 500px.
        /// </summary>
        [DefaultValue(500)]
        public Unit SliderWidth
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets whatever first,last, previous and next should be an option inside the dropdown.
        /// </summary>
        /// <value><c>true</c> if [compact drop down]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        internal bool CompactDropDown
        {
            get; set;
        }

        internal string GetPager
        {
            get
            {
                if (pagercontent != null)
                    return pagercontent;
                StringBuilder builder = new StringBuilder();
                StringWriter theStringWriter = new StringWriter(builder);
                HtmlTextWriter theHtmlWriter = new HtmlTextWriter(theStringWriter);

                Render(theHtmlWriter);

                return pagercontent =  builder.ToString();
            }
        }

        internal string GetPagerStatus
        {
            get
            {
                if (PagerType == PagerType.None || m_PagerGrid.PageSize >= m_PagerGrid.RecordCount)
                    return string.Empty;
                if (PagerType == PagerType.RangedSlider)
                {
                    int[] values = GetSliderValues;
                    if (values != null && values.Length == 2)
                        return String.Format(m_PagerGrid.GetSystemMessage("PagerStatusSlider"), string.Empty,
                                             string.Empty,
                                             values[0].ToString("N0"),
                                             values[1].ToString("N0"),
                                             (m_PagerGrid.RecordCount + 1).ToString("N0"));
                    return string.Empty;
                }
                int startposition = 1;
                int stopposition;
                if (m_PagerGrid.PageIndex  == 0)
                {
                    stopposition = m_PagerGrid.RecordCount + 1;
                    m_Pages = 1;
                }
                else
                {
                    if (m_PagerGrid.RecordCount > 0)
                        m_Pages = m_PagerGrid.RecordCount/m_PagerGrid.PageSize;
                    if (m_PagerGrid.RecordCount % m_PagerGrid.PageSize != 0)
                        m_Pages++;
                    m_PagerGrid.PageIndex = m_PagerGrid.PageIndex;
                    startposition = (m_PagerGrid.PageIndex  - 1)*m_PagerGrid.PageSize;
                    startposition = startposition < 0 ? 1 : startposition + 1;
                    stopposition = m_PagerGrid.PageIndex *m_PagerGrid.PageSize;
                    if (stopposition > m_PagerGrid.RecordCount + 1)
                        stopposition = m_PagerGrid.RecordCount + 1;
                }
                return
                    String.Format(m_PagerGrid.GetSystemMessage("PagerStatus"), m_PagerGrid.PageIndex , m_Pages,
                                  startposition.ToString("N0"),
                                  stopposition.ToString("N0"), (m_PagerGrid.RecordCount + 1).ToString("N0"));
            }
        }

        internal int NumberOfPagersToGenerate
        {
            get
            {
                switch (NavigateStyle)
                {
                    case NavigateStyle.Paging:
                        return PagerSequence*PagerWidth;
                    default:
                         if (m_PagerGrid.PageIndex <= PagerWidth / 2)
                            return Convert.ToInt32(Math.Floor(PagerWidth/(float) 2))*2 + 1;
                        return (PagerWidth/2) + m_PagerGrid.PageIndex ;
                }
            }
        }

        /// <summary>
        /// Sets or gets the slider width used inside this grid. Default size is 500px.
        /// </summary>
        [DefaultValue(Orientation.Horizontal)]
        internal Orientation SliderOperation
        {
            get; set;
        }

        private int PagerSequence
        {
            get
            {
                return NavigateStyle == NavigateStyle.Paging
                           ? Convert.ToInt32
                                 (Math.Floor((m_PagerGrid.PageIndex  - 1)/(double) PagerWidth)) + 1
                           : Convert.ToInt32
                                 (Math.Ceiling(m_PagerGrid.PageIndex /(double) PagerWidth));
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //There is nothing to clean.
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
        internal void Render(TextWriter writer)
        {
            if (PagerType == PagerType.None)
                return;
            if (m_EventRanpreRender == false)
                RenderControl(writer);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void RenderControl(TextWriter writer)
        {
            if (m_EventRanpreRender)
                return;
            m_EventRanpreRender = true;

            if (PagerType == PagerType.None || m_PagerGrid.PageSize >= m_PagerGrid.RecordCount)
                return;

            m_Pages = m_PagerGrid.PageSize == 0 ? 1 : (m_PagerGrid.RecordCount / m_PagerGrid.PageSize) ;
            if (m_PagerGrid.RecordCount % m_PagerGrid.PageSize != 0)
                m_Pages++;
            PreviousTemplate = string.Format(PreviousTemplate, ImagePath, m_PagerGrid.GetSystemMessage("Prev"));
            NextTemplate = string.Format(NextTemplate, ImagePath, m_PagerGrid.GetSystemMessage("Next"));
            FirstTemplate = string.Format(FirstTemplate, ImagePath, m_PagerGrid.GetSystemMessage("First"));
            LastTemplate = string.Format(LastTemplate, ImagePath, m_PagerGrid.GetSystemMessage("Last"));

            writer.Write("<table class=\"wgpager\"><tr>");
            if (PagerType != PagerType.RangedSlider)
            {
                if (DisplayFirstLast /*&& !(PagerType == PagerType.DropDown && CompactDropDown)*/)
                    writer.Write("<td>{0}</td>",
                                 Buttons.Anchor(m_PagerGrid, FirstTemplate, "PagerClick", new[] {"1"}, null,
                                                m_PagerGrid.GetSystemMessage("First"),
                                                m_PagerGrid.GetSystemMessageClass("First", "ui-icon ui-icon-seek-first"),
                                                null, false));
                if (DisplayPreviousNext /*&& !(PagerType == PagerType.DropDown && CompactDropDown)*/)
                    writer.Write("<td>{0}</td>",
                                 Buttons.Anchor(m_PagerGrid, PreviousTemplate, "PagerClick",
                                                new[] { (m_PagerGrid.PageIndex  - 1).ToString() }, null,
                                                m_PagerGrid.GetSystemMessage("Prev"),
                                                "ui-icon ui-icon-seek-prev", null, false));
            }
            switch (PagerType)
            {
                case PagerType.DropDown:
                    writer.Write("<td>");
                    writer.Write(CreateDropDownPager(DisplayAll));
                    writer.Write("</td>");
                    break;
                case PagerType.Slider:
                case PagerType.RangedSlider:
                    writer.Write("<td>");
                    writer.Write(CreateSlider());
                    writer.Write("</td>");
                    break;
                default:
                    writer.Write(CreateBarPager(DisplayAll));
                    break;
            }
            if (PagerType != PagerType.RangedSlider)
            {
                if (DisplayPreviousNext
                    /*&& !(PagerType == PagerType.DropDown && CompactDropDown)*/)
                {
                    int next = (m_PagerGrid.PageIndex  + 1);
                    if (next > m_Pages)
                        next = m_Pages;
                    writer.Write("<td>{0}</td>",
                                 Buttons.Anchor(m_PagerGrid, NextTemplate, "PagerClick", new[] {(next).ToString()},
                                                null,
                                                m_PagerGrid.GetSystemMessage("Next"),
                                                m_PagerGrid.GetSystemMessageClass("Next", "ui-icon ui-icon-seek-next"),
                                                null,
                                                false));
                }
                if (DisplayFirstLast
                    /*&& !(PagerType == PagerType.DropDown && CompactDropDown)*/)
                    writer.Write("<td>{0}</td>", Buttons.Anchor(m_PagerGrid, LastTemplate, "PagerClick",
                                                                new[] {m_Pages.ToString()}, null,
                                                                m_PagerGrid.GetSystemMessage("Last"),
                                                                m_PagerGrid.GetSystemMessageClass("Last",
                                                                                                  "ui-icon ui-icon-seek-end"),
                                                                null, false));
            }
            writer.Write("</tr></table>");
        }

        internal void updateSliderValues()
        {
            if (!Grid.GotHttpContext || HttpContext.Current.Request[m_PagerGrid.ID + "slidervalue"] == null)
                return;
            m_PagerGrid.PagerSettings.SliderValue = HttpContext.Current.Request[m_PagerGrid.ID + "slidervalue"];
            int[] values = GetSliderValues;
            if (values != null)
                switch (values.Length)
                {
                        //Ranged slider
                    case 2:
                        {
                            m_PagerGrid.PageIndex = 1;
                            m_PagerGrid.PageSize = values[1];

                        }
                        break;
                        //Page slider
                    case 1:
                        {
                            m_PagerGrid.PageIndex = values[0];
                        }
                        break;
                }
        }

        private string CreateBarPager(bool all)
        {
            StringBuilder bar = new StringBuilder(string.Empty);

            string preFix = PageBarTemplate.Substring(0, PageBarTemplate.IndexOf("{0}"));
            string postFix = PageBarTemplate.Substring(PageBarTemplate.IndexOf("{0}") + 3);
            string selectedPreFix = SelectedPageBarTemplate.Substring(0, SelectedPageBarTemplate.IndexOf("{0}"));
            string selectedPostFix = SelectedPageBarTemplate.Substring(SelectedPageBarTemplate.IndexOf("{0}") + 3);

            int startIndex;

            if (NavigateStyle == NavigateStyle.Paging)
                startIndex = (PagerSequence - 1)*PagerWidth + 1;
            else
            {
                startIndex = m_PagerGrid.PageIndex  - PagerWidth/2;
                if (startIndex + PagerWidth > m_Pages)
                    startIndex = m_Pages - PagerWidth + 1;

                if (startIndex < 1)
                    startIndex = 1;
            }

            if (all)
            {
                string records = string.Format(" ({0:N0})", m_PagerGrid.RecordCount + 1);
                if (m_PagerGrid.PageIndex  == 0)
                {
                    bar.AppendFormat("<td>{0}<span title=\"{2}\" class=\"wgpagerselected\">{3}{4}</span>{1}</td>",
                                     selectedPreFix, selectedPostFix, m_PagerGrid.GetSystemMessage("AllTitle"),
                                     m_PagerGrid.GetSystemMessage("All"), records);
                }
                else
                {
                    string textcontent = m_PagerGrid.RecordCount < 500
                                             ? Buttons.Anchor(m_PagerGrid, m_PagerGrid.GetSystemMessage("All") + records,
                                                              "PagerClick",
                                                              new[] {"ALL"}, null,
                                                              m_PagerGrid.GetSystemMessage("AllTitle"),
                                                              m_PagerGrid.GetSystemMessageClass("All", "wgpagerall"),
                                                              null, true)
                                             : Buttons.Anchor(m_PagerGrid, m_PagerGrid.GetSystemMessage("All") + records,
                                                              "PagerClick",
                                                              new[] {"ALL"},
                                                              string.Format(
                                                                  m_PagerGrid.GetSystemMessage("DisplayReords"),
                                                                  m_PagerGrid.RecordCount.ToString("N0")),
                                                              m_PagerGrid.GetSystemMessage("AllTitle"),
                                                              m_PagerGrid.GetSystemMessageClass("All", "wgpagerall"),
                                                              null, true);

                    bar.AppendFormat("<td>{0}{1}{2}</td>", preFix, textcontent, postFix);
                }
            }
            for (int i = startIndex; i <= NumberOfPagersToGenerate && i <= m_Pages; i++)
            {
                string strI = i.ToString();
                if (CurrentPageDigits > 0)
                    strI = strI.PadLeft(CurrentPageDigits, "0"[0]);

                if (i != m_PagerGrid.PageIndex )
                {
                    bar.AppendFormat("<td>{0}{1}{2}</td>", preFix,
                                     Buttons.Anchor(m_PagerGrid, strI, "PagerClick", new[] {i.ToString()}, null,
                                                    String.Format("{0}/{1}", i, m_Pages), "wgpagerunselected", null,
                                                    true), postFix);
                }
                else
                {
                    if (m_PagerGrid.IsUsingJQueryUICSSFramework)
                        bar.AppendFormat(
                            "<td>{0}<span class=\"wgpagerselected\"><button class=\"ui-button ui-state-default ui-state-disabled ui-corner-all\">{1}</button></span>{2}</td>",
                            selectedPreFix, strI,
                            selectedPostFix);
                    else
                        bar.AppendFormat("<td>{0}<span class=\"wgpagerselected\">{1}</span>{2}</td>", selectedPreFix,
                                         strI,
                                         selectedPostFix);
                }
            }
            return bar.ToString();
        }

        private string CreateDropDownPager(bool all)
        {
            string eventScript = null;

            if(m_PagerGrid.Page != null )
            {

                eventScript = m_PagerGrid.EnableCallBack
                    ? Asynchronous.GetCallbackEventReference(m_PagerGrid, "PagerClick!JS", false,
                                                             string.Empty, string.Empty)
                    : m_PagerGrid.Page.ClientScript.GetPostBackEventReference(m_PagerGrid, "PagerClick![JS]");
                eventScript = eventScript.Replace("JS", "' + this.value + '");
            }
            StringBuilder dropDown = new StringBuilder(string.Format("<select onchange=\"{0}\" >", eventScript));

            if (all)
                dropDown.AppendFormat("<option value=\"ALL\">{0}</option>", m_PagerGrid.GetSystemMessage("All"));

            if (CompactDropDown && DisplayFirstLast)
                dropDown.AppendFormat("<option value=\"1\">{0}</option>", m_PagerGrid.GetSystemMessage("First"));
            if (CompactDropDown && DisplayPreviousNext)
                dropDown.AppendFormat("<option value=\"{0}\">{1}</option>", (m_PagerGrid.PageIndex  + 1),
                                      m_PagerGrid.GetSystemMessage("Prev"));

            const int startIndex = 1;
            /*
                if( NavigateStyle == NavigateStyles.Paging )
                    startIndex = (PagerSequence-1)* PagerWidth+1;
                else
                {
                    startIndex = m_PagerGrid.PageIndex  - PagerWidth/2;
                    if(startIndex + PagerWidth > pages)
                        startIndex = pages - PagerWidth + 1;

                    if(startIndex < 1 )
                        startIndex = 1;
                }
                */
            for (int i = startIndex; i <= m_Pages; i++)
            {
                string selected = (i == m_PagerGrid.PageIndex ) ? "selected=\"selected\"" : string.Empty;

                dropDown.AppendFormat("<option {0} value=\"{1}\">{2}</option>", selected, i,
                                      String.Format(PageDropDownTemplate, i, m_Pages));
            }

            if (CompactDropDown && DisplayPreviousNext)
            {
                int next = (m_PagerGrid.PageIndex  + 1);
                if (next > m_Pages)
                    next = m_Pages;

                dropDown.AppendFormat("<option value={0}>{1}</option>", next, m_PagerGrid.GetSystemMessage("Next"));
            }
            if (CompactDropDown && DisplayFirstLast)
                dropDown.AppendFormat("<option value={0}>{1}</option>", m_Pages, m_PagerGrid.GetSystemMessage("Last"));

            dropDown.Append("</select>");
            return dropDown.ToString();
        }

        private string CreateSlider()
        {
            int startrecord;
            int endrecord = 0;

           

            string range;
            string slidertemplate;
            int recordcount;

            if (PagerType == PagerType.RangedSlider)
            {
                range = "true";
                startrecord = m_PagerGrid.PageIndex * m_PagerGrid.PageSize;
                recordcount = m_PagerGrid.RecordCount;
                slidertemplate = gridsliderrange;
                endrecord = (m_PagerGrid.PageIndex * m_PagerGrid.PageSize) + m_PagerGrid.PageSize;
            }
            else
            {
                startrecord = m_PagerGrid.PageIndex;
                range = "false";
                recordcount = m_Pages;
                slidertemplate = gridslider;
            }

            int[] values = GetSliderValues;
            if (values != null)
                switch (values.Length)
                {
                    //Ranged slider
                    case 2:
                        startrecord = values[0];
                        endrecord = values[1];
                        break;
                    //Page slider
                    case 1:
                        startrecord = values[0];
                        break;
                }
            if (Anthem.Manager.IsCallBack)
                Anthem.Manager.AddScriptForClientSideEval(string.Format(slidertemplate, startrecord, endrecord,

                                                                        recordcount,
                                                                        m_PagerGrid.ID, SliderStep,
                                                                       range,
                                                                        m_PagerGrid.ClientObjectId ?? m_PagerGrid.ID,SliderOperation.ToString().ToLower()));
            else
                m_PagerGrid.Page.ClientScript.RegisterClientScriptBlock(m_PagerGrid.GetType(),
                                                                        m_PagerGrid.ID,
                                                                        string.Format(slidertemplate, startrecord,
                                                                                      endrecord,

                                                                                      recordcount,
                                                                                      m_PagerGrid.ID,
                                                                                      SliderStep,
                                                                                     range,
                                                                                      m_PagerGrid.ClientObjectId ??
                                                                                      m_PagerGrid.ID, SliderOperation.ToString().ToLower()), true);

            switch (SliderOperation)
            {
                case Orientation.Horizontal:
                    if (Grid.GotHttpContext)
                        return
                            string.Format(
                                "<input class=\"wgslidervalue\"  type=\"hidden\" name=\"{0}slidervalue\" id=\"{0}slidervalue\"  /><div class=\"wgslider\" style=\"width:{2}\" id=\"{0}slider\" title=\"{1}\" />",
                                m_PagerGrid.ID,
                                HttpContext.Current.Server.HtmlEncode(GetPagerStatus), SliderWidth);
                    return
                        string.Format(
                            "<input class=\"wgslidervalue\" type=\"hidden\" name=\"{0}slidervalue\" id=\"{0}slidervalue\"  /><div class=\"wgslider\" style=\"width:{2}\" id=\"{0}slider\" title=\"{1}\" />",
                            m_PagerGrid.ID, GetPagerStatus, SliderWidth);
                case Orientation.Vertical:
                    if (Grid.GotHttpContext)
                        return
                            string.Format(
                                "<input class=\"wgslidervalue\"  type=\"hidden\" name=\"{0}slidervalue\" id=\"{0}slidervalue\"  /><div class=\"wgslider\" style=\"height:{2}\" id=\"{0}slider\" title=\"{1}\" />",
                                m_PagerGrid.ID,
                                HttpContext.Current.Server.HtmlEncode(GetPagerStatus), SliderHeight);
                    return
                        string.Format(
                            "<input class=\"wgslidervalue\" type=\"hidden\" name=\"{0}slidervalue\" id=\"{0}slidervalue\"  /><div class=\"wgslider\" style=\"height:{2}\" id=\"{0}slider\" title=\"{1}\" />",
                            m_PagerGrid.ID, GetPagerStatus, SliderHeight);

            }
            return null;
        }

        /// <summary>
        /// Initializes default properties for the navigation pager.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void InitProperties(Grid parent)
        {
            m_PagerGrid = parent;
            DisplayPreviousNext = false;
            DisplayFirstLast = true;
            DisplayAll = false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSite()
        {
            return false;
        }

        #endregion Methods
    }
}