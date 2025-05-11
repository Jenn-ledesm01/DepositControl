using PdfText = iTextSharp.text;

namespace DepositControl
{
        using System;
        using System.IO;

        using iTextSharp.text.pdf;

        using PdfText;
        //utilidad
        public class PdfUtility
        {
            public class HeaderFooterNovedades : PdfPageEventHelper
            {

                // This is the contentbyte object of the writer
                private PdfContentByte _cb;

                // we will put the final number of pages in a template
                private PdfTemplate _headerTemplate, _footerTemplate;

                // this is the BaseFont we are going to use for the header / footer
                private BaseFont _bf;

                // This keeps track of the creation time
                private DateTime _printTime = DateTime.Now;

                #region Fields

                #endregion

                #region Properties

                public string Header { get; set; }
                public string UserName { get; set; }
                public string ImageUrl { get; set; }
                public string FechaNovedad { get; set; }
                public string DateDesde { get; set; }
                public string DateHasta { get; set; }
                public string DocType { get; set; }
                public string TitlePage { get; set; }
                public int PageNumber { get; set; }
                public DateTime PrintTime
                {
                    get { return this._printTime; }
                    set { this._printTime = value; }
                }

                #endregion

                public override void OnOpenDocument(PdfWriter writer, Document document)
                {
                    try
                    {
                        this._printTime = DateTime.Now;
                        this._bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        this._cb = writer.DirectContent;
                        this._headerTemplate = this._cb.CreateTemplate(100, 100);
                        this._footerTemplate = this._cb.CreateTemplate(50, 50);
                    }
                    catch (DocumentException de)
                    {
                        throw de;
                    }
                    catch (IOException ioe)
                    {
                        throw ioe;
                    }
                }
                public override void OnEndPage(PdfWriter writer, Document document)
                {
                    base.OnEndPage(writer, document);

                    Font brown = FontFactory.GetFont("arial", 24f, new BaseColor(163, 21, 21));
                    Font negroArial = FontFactory.GetFont("arial", 16f, new BaseColor(0, 0, 0));
                    Font negro2Arial = FontFactory.GetFont("arial", 14f, new BaseColor(0, 0, 0));
                    //Font georgia2 = FontFactory.GetFont("arial", 10f, BaseColor.WHITE);
                    Font georgia3 = FontFactory.GetFont("arial", 9f, BaseColor.BLACK);
                    PageNumber = writer.PageNumber;
                    String text = "Page " + writer.PageNumber + " of ";


                    //Add paging to footer
                    {
                        this._cb.BeginText();
                        this._cb.SetFontAndSize(this._bf, 9);
                        this._cb.SetTextMatrix(500, 25);
                        //cb.SetTextMatrix(document.PageSize.GetRight(180), document.PageSize.GetBottom(30));
                        this._cb.ShowText(text);
                        this._cb.EndText();
                        float len = this._bf.GetWidthPoint(text, 9);
                        //cb.AddTemplate(footerTemplate, document.PageSize.GetRight(180) + len, document.PageSize.GetBottom(30));
                        this._cb.AddTemplate(this._footerTemplate, 500 + len, 25);
                    }

                    //Create PdfTable object
                    PdfPTable pdfTab = new PdfPTable(2) { HeaderRows = 0, WidthPercentage = 98 };
                    pdfTab.DefaultCell.Border = 1;

                    //We will have to create separate cells to include image logo and 2 separate strings
                    //Row 1
                    PdfPCell pdfCell1 = new PdfPCell();
                    Image fI = Image.GetInstance(this.ImageUrl);
                    fI.ScalePercent(60);
                    pdfCell1.AddElement(fI);

                    //Row 2
                    string fe = "Date " + DateTime.Now.ToString("yy-MM-dd") + " " + DateTime.Now.ToString("hh:mm");
                    PdfPCell pdfCell2 = new PdfPCell(new Phrase(fe, georgia3));
                    pdfCell2.Phrase.Add(new Chunk(Environment.NewLine, georgia3));
                    pdfCell2.Phrase.Add(new Chunk(this.UserName, georgia3));

                    if (this.DocType != "N")
                    {
                        //Row 3
                        const string tituloPdf = "PARTE DIARIO DE NOVEDADES - ";
                        Chunk c1 = new Chunk(Environment.NewLine, brown);
                        Chunk c2 = new Chunk(tituloPdf + " " + "Date " + this.FechaNovedad, negroArial);
                        Chunk c3 = new Chunk(Environment.NewLine);
                        Phrase p1 = new Phrase { c1, c2, c3 };
                        Paragraph p = new Paragraph { Alignment = 1 };
                        p.Add(p1);
                        PdfPCell pdfCell3 = new PdfPCell();
                        pdfCell3.AddElement(p);

                        //Row 4
                        Chunk a2 = new Chunk("Sector: CPD ", negro2Arial);
                        Chunk a3 = new Chunk(Environment.NewLine);
                        Chunk a4 = new Chunk(Environment.NewLine, brown);
                        Phrase pa1 = new Phrase { a2, a3, a4 };
                        Paragraph o = new Paragraph { IndentationLeft = 15f };
                        o.Add(pa1);
                        PdfPCell pdfCell4 = new PdfPCell();
                        pdfCell4.AddElement(o);

                        pdfCell2.HorizontalAlignment = Element.ALIGN_RIGHT;

                        pdfCell4.HorizontalAlignment = Element.ALIGN_LEFT;

                        pdfCell3.Colspan = 2;
                        pdfCell4.Colspan = 2;

                        pdfCell1.Border = 0;
                        pdfCell2.Border = 0;
                        pdfCell3.Border = 0;
                        pdfCell4.Border = 0;

                        //add all three cells into PdfTable
                        pdfTab.AddCell(pdfCell1);
                        pdfTab.AddCell(pdfCell2);
                        pdfTab.AddCell(pdfCell3);
                        pdfTab.AddCell(pdfCell4);

                        pdfTab.TotalWidth = document.PageSize.Width - 80f;
                        pdfTab.WidthPercentage = 70;
                        //pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;

                        //recuadro
                        PdfContentByte canvas = writer.DirectContent;
                        Rectangle rect1 = new Rectangle(550, 700, 50, 250) { Border = Rectangle.BOX, BorderWidth = 1 };
                        //550x 700y coordenada punto de inicio, 
                        //50 ancho del rectángulo y 250 alto del rectángulo

                        Rectangle rect2 = new Rectangle(450, 250, 550, 75) { Border = Rectangle.BOX, BorderWidth = 1 };
                        Rectangle rect3 = new Rectangle(450, 250, 550, 160) { Border = Rectangle.BOX, BorderWidth = 1 };

                        canvas.Rectangle(rect1);
                        canvas.Rectangle(rect2);
                        canvas.BeginText();
                        canvas.SetFontAndSize(this._bf, 12);
                        canvas.SetTextMatrix(485, 200);
                        canvas.ShowText("Jefe");
                        canvas.EndText();
                        canvas.Rectangle(rect3);
                    }
                    else
                    {
                        //Row 3
                        Chunk c1 = new Chunk(Environment.NewLine, brown);
                        Chunk c2 = new Chunk(this.TitlePage, negroArial);
                        //Chunk c3 = new Chunk(Environment.NewLine);
                        Phrase p1 = new Phrase { c1, c2 };
                        //p1.Add(c3);
                        Paragraph p = new Paragraph { Alignment = 2 };
                        p.Add(p1);
                        PdfPCell pdfCell3 = new PdfPCell();
                        pdfCell3.AddElement(p);

                        //Row 4
                        Chunk a2 = new Chunk("", negro2Arial);
                        Chunk a3 = new Chunk(Environment.NewLine);
                        Chunk a4 = new Chunk(Environment.NewLine, brown);
                        Phrase pa1 = new Phrase { a2, a3, a4 };
                        Paragraph o = new Paragraph { IndentationLeft = 15f };
                        o.Add(pa1);
                        PdfPCell pdfCell4 = new PdfPCell();
                        pdfCell4.AddElement(o);

                        pdfCell2.HorizontalAlignment = Element.ALIGN_RIGHT;

                        pdfCell4.HorizontalAlignment = Element.ALIGN_LEFT;


                        pdfCell3.Colspan = 2;
                        pdfCell4.Colspan = 2;

                        pdfCell1.Border = 0;
                        pdfCell2.Border = 0;
                        pdfCell3.Border = 0;
                        pdfCell4.Border = 0;

                        //add all three cells into PdfTable
                        pdfTab.AddCell(pdfCell1);
                        pdfTab.AddCell(pdfCell2);
                        pdfTab.AddCell(pdfCell3);
                        pdfTab.AddCell(pdfCell4);

                        pdfTab.TotalWidth = document.PageSize.Width - 80f;
                        pdfTab.WidthPercentage = 70;
                        //pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;

                    }

                    //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
                    //first param is start row. -1 indicates there is no end row and all the rows to be included to write
                    //Third and fourth param is x and y position to start writing
                    pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
                    //set pdfContent value

                    //Move the pointer and draw line to separate header section from rest of page
                    //cb.MoveTo(40, document.PageSize.Height - 100);
                    //cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
                    //cb.Stroke();

                    //Move the pointer and draw line to separate footer section from rest of page
                    //cb.MoveTo(40, document.PageSize.GetBottom(50));
                    //cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
                    //cb.Stroke();
                }
                public override void OnCloseDocument(PdfWriter writer, Document document)
                {
                    base.OnCloseDocument(writer, document);

                    this._headerTemplate.BeginText();
                    this._headerTemplate.SetFontAndSize(this._bf, 9);
                    this._headerTemplate.SetTextMatrix(0, 0);
                    this._headerTemplate.ShowText((PageNumber).ToString());
                    this._headerTemplate.EndText();

                    this._footerTemplate.BeginText();
                    this._footerTemplate.SetFontAndSize(this._bf, 9);
                    this._footerTemplate.SetTextMatrix(0, 0);
                    this._footerTemplate.ShowText((PageNumber).ToString());
                    this._footerTemplate.EndText();
                }
            }

            public class HeaderFooterReportes : PdfPageEventHelper
            {


                // This is the contentbyte object of the writer
                private PdfContentByte _cb;

                // we will put the final number of pages in a template
                private PdfTemplate _headerTemplate, _footerTemplate;

                // this is the BaseFont we are going to use for the header / footer
                private BaseFont _bf;

                // This keeps track of the creation time
                private DateTime _printTime = DateTime.Now;
                public int PageNumber { get; set; }
                #region Fields

                private const int HeaderWidth = 50;
                private const int HeaderHeight = 100;
                private const int FooterWidth = 50;
                private const int FooterHeight = 50;

                #endregion

                #region Properties

                public string Header { get; set; }
                public string UserName { get; set; }
                public string ImageUrl { get; set; }
                public string FilterDate { get; set; }
                public string FilterSucursal { get; set; }
                public string FilterTeamLeader { get; set; }
                public string FilterComercializadora { get; set; }
                public string TitlePage { get; set; }

                public DateTime PrintTime
                {
                    get { return this._printTime; }
                    set { this._printTime = value; }
                }

                #endregion

                public override void OnOpenDocument(PdfWriter writer, Document document)
                {
                    try
                    {
                        this._printTime = DateTime.Now;
                        this._bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        this._cb = writer.DirectContent;
                        this._headerTemplate = this._cb.CreateTemplate(HeaderWidth, HeaderHeight);
                        this._footerTemplate = this._cb.CreateTemplate(FooterWidth, FooterHeight);
                    }
                    catch (DocumentException de)
                    {
                        throw de;
                    }
                    catch (IOException ioe)
                    {
                        throw ioe;
                    }
                }
                public override void OnStartPage(PdfWriter writer, Document document)
                {
                    base.OnStartPage(writer, document);
                    Rectangle pageSize = document.PageSize;
                    Font brown = FontFactory.GetFont("arial", 24f, new BaseColor(163, 21, 21));
                    Font negroArial = FontFactory.GetFont("arial", 16f, new BaseColor(0, 0, 0));
                    Font negro2Arial = FontFactory.GetFont("arial", 14f, new BaseColor(0, 0, 0));
                    Font georgia2 = FontFactory.GetFont("arial", 10f, BaseColor.WHITE);
                    Font georgia3 = FontFactory.GetFont("arial", 9f, BaseColor.BLACK);


                    //Create PdfTable object
                    PdfPTable pdfTab = new PdfPTable(3) { HeaderRows = 0, WidthPercentage = 98 };
                    pdfTab.DefaultCell.Border = 0;
                    pdfTab.TotalWidth = document.PageSize.Width - 80f;
                    pdfTab.WidthPercentage = 70;

                    //Row 1
                    PdfPCell pdfCell1 = new PdfPCell();
                    Image fI = Image.GetInstance(this.ImageUrl);
                    fI.ScalePercent(60);
                    pdfCell1.AddElement(fI);

                    PdfPCell pdfCell3 = new PdfPCell(new Phrase(this.TitlePage, negroArial));
                    pdfCell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfCell3.Border = 0;
                    //Row 2
                    string fe = $"{Res.res.dateofconsultation}: {DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm")}";
                    PdfPCell pdfCell2 = new PdfPCell(new Phrase(fe, georgia3));
                    pdfCell2.Phrase.Add(new Chunk(Environment.NewLine, georgia3));
                    pdfCell2.Phrase.Add(new Chunk(this.UserName, georgia3));

                    pdfCell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pdfCell1.Border = 0;
                    pdfCell2.Border = 0;

                    pdfTab.AddCell(pdfCell1);
                    pdfTab.AddCell(pdfCell3);
                    pdfTab.AddCell(pdfCell2);

                    pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 20, this._cb);

                    //Move the pointer and draw line to separate header section from rest of page
                    this._cb.MoveTo(40, document.PageSize.Height - 60);
                    //_cb.LineTo(pageSize.GetLeft(40), pageSize.GetTop(50));
                    this._cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 60);
                    this._cb.SetLineWidth(1);
                    this._cb.SetColorStroke(new BaseColor(27, 78, 120));
                    this._cb.Stroke();

                    //Move the pointer and draw line to separate footer section from rest of page
                    this._cb.MoveTo(40, document.PageSize.GetBottom(50));
                    this._cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
                    this._cb.Stroke();
                }
                public override void OnEndPage(PdfWriter writer, Document document)
                {
                    base.OnEndPage(writer, document);
                    int pageN = writer.PageNumber;

                    String text = "Page " + pageN + " of ";
                    PageNumber = pageN;
                    float len = this._bf.GetWidthPoint(text, 9);
                    Rectangle pageSize = document.PageSize;


                    //Add paging to footer
                    {
                        this._cb.BeginText();
                        this._cb.SetFontAndSize(this._bf, 9);
                        this._cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
                        this._cb.ShowText(text);
                        this._cb.EndText();
                        this._cb.AddTemplate(this._footerTemplate, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));

                    }
                }
                public override void OnCloseDocument(PdfWriter writer, Document document)
                {
                    base.OnCloseDocument(writer, document);
                    this._footerTemplate.BeginText();
                    this._footerTemplate.SetFontAndSize(this._bf, 9);
                    this._footerTemplate.SetTextMatrix(0, 0);
                    this._footerTemplate.ShowText((PageNumber).ToString());
                    this._footerTemplate.EndText();
                }
            }

            public class TableBuilder
            {
                // createTable
                public static PdfPTable CreateTable(int columnNumber)
                {
                    // create num of column table
                    PdfPTable table = new PdfPTable(columnNumber) { WidthPercentage = 100 }; // set the width of the table to 100% of page

                    // set relative columns width
                    table.SetWidths(new float[] { 0.6f, 1.4f, 0.8f, 0.8f, 1.8f, 2.6f });

                    // ----------------Table Header "Title"----------------
                    // font
                    Font font = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD, BaseColor.WHITE);
                    // create header cell
                    PdfPCell cell = new PdfPCell(new Phrase("HMKCODE.com - iText PDFTable Example", font))
                    {
                        Colspan = (columnNumber) // set Column span "1 cell = 6 cells width"
                    };
                    // set style
                    Style.HeaderCellStyle(cell);
                    // add to table
                    table.AddCell(cell);

                    //-----------------Table Cells Label/Value------------------
                    //// 1st Row
                    //table.AddCell(CreateLabelCell("Label 1"));
                    //table.AddCell(CreateValueCell("Value 1"));
                    //table.AddCell(CreateLabelCell("Label 2"));
                    //table.AddCell(CreateValueCell("Value 2"));
                    //table.AddCell(CreateLabelCell("Label 3"));
                    //table.AddCell(CreateValueCell("Value 3"));
                    //// 2nd Row
                    //table.AddCell(CreateLabelCell("Label 4"));
                    //table.AddCell(CreateValueCell("Value 4"));
                    //table.AddCell(CreateLabelCell("Label 5"));
                    //table.AddCell(CreateValueCell("Value 5"));
                    //table.AddCell(CreateLabelCell("Label 6"));
                    //table.AddCell(CreateValueCell("Value 6"));

                    return table;
                }
                // createCells
                public static PdfPCell CreateLabelCell(String text, String type)
                {
                    // font
                    Font font = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.DARK_GRAY);
                    Font arialFont = FontFactory.GetFont("arial", 9f, BaseColor.BLACK);
                    Font arialBoldFont = FontFactory.GetFont("arial", 9f, 1, BaseColor.BLACK);

                    // create cell
                    PdfPCell cell = new PdfPCell(new Phrase(text, arialFont));

                    if (type == "T")
                    {
                        // set style
                        Style.LabelTotalCellStyle(cell);
                    }
                    else
                    {
                        // set style
                        Style.LabelCellStyle(cell);
                    }

                    return cell;
                }
                // createCells
                public static PdfPCell CreateValueCell(String text, String type)
                {
                    // font
                    Font font = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                    Font arialFont = FontFactory.GetFont("arial", 9f, BaseColor.BLACK);
                    Font arialBoldFont = FontFactory.GetFont("arial", 9f, 1, BaseColor.BLACK);

                    // create cell
                    PdfPCell cell = new PdfPCell(new Phrase(text, arialFont));

                    if (type == "T")
                    {
                        // set style
                        Style.ValueTotalCellStyle(cell);
                    }
                    else
                    {
                        // set style
                        Style.ValueCellStyle(cell);
                    }

                    return cell;
                }
            }
            public class Style
            {
                public static void HeaderCellStyle(PdfPCell cell)
                {
                    // alignment
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    // padding
                    cell.PaddingTop = 0f;
                    cell.PaddingBottom = 7f;

                    // background color
                    cell.BackgroundColor = new BaseColor(0, 121, 182);

                    // border
                    cell.Border = 0;
                    cell.BorderWidthBottom = 2f;
                }
                public static void LabelCellStyle(PdfPCell cell)
                {
                    // alignment
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_CENTER;

                    // padding
                    cell.PaddingLeft = 3f;
                    cell.PaddingTop = 0f;

                    // background color
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;

                    // border
                    cell.Border = 0;
                    cell.BorderWidthBottom = 1;
                    cell.BorderColorBottom = BaseColor.GRAY;

                    // height
                    cell.MinimumHeight = 18f;
                }
                public static void LabelTotalCellStyle(PdfPCell cell)
                {
                    // alignment
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_CENTER;

                    // background color
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;

                    // height
                    cell.MinimumHeight = 18f;
                }
                public static void ValueCellStyle(PdfPCell cell)
                {
                    // alignment
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_CENTER;

                    // padding
                    //cell.PaddingTop = 0f;
                    //cell.PaddingBottom = 5f;

                    //// border
                    //cell.Border = 0;
                    //cell.BorderWidthBottom = 0.5f;

                    // height
                    cell.MinimumHeight = 18f;
                }
                public static void ValueTotalCellStyle(PdfPCell cell)
                {
                    // alignment
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_CENTER;

                    // background color
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;

                    // height
                    cell.MinimumHeight = 18f;
                }
            }
        }
    }
