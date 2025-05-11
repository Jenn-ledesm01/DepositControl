using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;


namespace DepositControl
{
    /// <summary>
    /// Para utilizar se debe crear la instancia de la clase pasando como parametro la 
    /// instancia de XSSFWorkbook. Ejemplo:
    ///                                 var workbook = new XSSFWorkbook();
    ///                                 var excelUtil = new ExcelUtility(workbook);
    /// Luego se crean las hojas con los metodos "CreateSheet" o "CreateSheetAcumulado" segun sea el caso.
    /// Por ultimo se agregan las celdas con el metodo "CreateCell"
    /// </summary>
    public class ExcelUtility
    {
        #region atributos fuentes y estilos
        public IFont FontBold;
        public IFont FontBold18;
        public IFont FontBold14;
        public IFont Font;
        public ICellStyle lCellStyleTituloReporteTitulo;
        public ICellStyle lCellStyleTituloReporteFecha;
        public ICellStyle headerL;
        public ICellStyle headerC;
        public ICellStyle headerR;
        public ICellStyle rowLdouble;
        public ICellStyle rowCdouble;
        public ICellStyle rowRdouble;
        public ICellStyle rowLint;
        public ICellStyle rowCint;
        public ICellStyle rowRint;
        public ICellStyle rowLstring;
        public ICellStyle rowCstring;
        public ICellStyle rowRstring;
        public ICellStyle totalLstring;
        public ICellStyle totalCstring;
        public ICellStyle totalRstring;
        public ICellStyle totalLdouble;
        public ICellStyle totalCdouble;
        public ICellStyle totalRdouble;
        public ICellStyle totalLint;
        public ICellStyle totalCint;
        public ICellStyle totalRint;
        #endregion
        public XSSFWorkbook workbook;


        /// <summary>
        /// Constructor de la clase, inicializa los estilos para las celdas.
        /// </summary>
        /// <param name="workbookT">instancias de XSSFWorkbook</param>
        public ExcelUtility(XSSFWorkbook workbookT)
        {
            this.workbook = workbookT;

            #region estilos

            //fuente negrita
            FontBold = workbook.CreateFont();
            FontBold.Boldweight = (short)FontBoldWeight.Bold;

            //fuente negrita 18
            FontBold18 = workbook.CreateFont();
            FontBold18.Boldweight = (short)FontBoldWeight.Bold;
            FontBold18.FontHeightInPoints = (short)18;

            //fuente negrita 14
            FontBold14 = workbook.CreateFont();
            FontBold14.Boldweight = (short)FontBoldWeight.Bold;
            FontBold14.FontHeightInPoints = (short)14;

            //fuente normal
            Font = workbook.CreateFont();
            Font.Boldweight = (short)FontBoldWeight.None;

            //estilo Titulos reporte derecho
            lCellStyleTituloReporteTitulo = workbook.CreateCellStyle();
            lCellStyleTituloReporteTitulo.SetFont(FontBold18);
            lCellStyleTituloReporteTitulo.WrapText = true;
            lCellStyleTituloReporteTitulo.Alignment = HorizontalAlignment.Center;
            lCellStyleTituloReporteTitulo.VerticalAlignment = VerticalAlignment.Center;

            //estilo Titulos reporte izquierdo
            lCellStyleTituloReporteFecha = workbook.CreateCellStyle();
            lCellStyleTituloReporteFecha.SetFont(FontBold14);
            lCellStyleTituloReporteFecha.WrapText = true;
            lCellStyleTituloReporteFecha.Alignment = HorizontalAlignment.Left;
            lCellStyleTituloReporteFecha.VerticalAlignment = VerticalAlignment.Center;


            //estilo header
            headerL = workbook.CreateCellStyle();
            headerL.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            headerL.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            headerL.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            headerL.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            headerL.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            headerL.FillPattern = FillPattern.SolidForeground;
            headerL.SetFont(FontBold);
            headerL.WrapText = true;
            headerL.Alignment = HorizontalAlignment.Left;
            headerL.VerticalAlignment = VerticalAlignment.Center;

            headerC = workbook.CreateCellStyle();
            headerC.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            headerC.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            headerC.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            headerC.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            headerC.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            headerC.FillPattern = FillPattern.SolidForeground;
            headerC.SetFont(FontBold);
            headerC.WrapText = true;
            headerC.Alignment = HorizontalAlignment.Center;
            headerC.VerticalAlignment = VerticalAlignment.Center;

            headerR = workbook.CreateCellStyle();
            headerR.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            headerR.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            headerR.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            headerR.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            headerR.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            headerR.FillPattern = FillPattern.SolidForeground;
            headerR.SetFont(FontBold);
            headerR.WrapText = true;
            headerR.Alignment = HorizontalAlignment.Right;
            headerR.VerticalAlignment = VerticalAlignment.Center;


            //estilos row double
            rowLdouble = workbook.CreateCellStyle();
            rowLdouble.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            rowLdouble.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLdouble.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLdouble.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLdouble.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLdouble.WrapText = true;
            rowLdouble.Alignment = HorizontalAlignment.Left;
            rowLdouble.VerticalAlignment = VerticalAlignment.Center;
            rowLdouble.SetFont(Font);

            rowCdouble = workbook.CreateCellStyle();
            rowCdouble.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            rowCdouble.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCdouble.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCdouble.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCdouble.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCdouble.WrapText = true;
            rowCdouble.Alignment = HorizontalAlignment.Center;
            rowCdouble.VerticalAlignment = VerticalAlignment.Center;
            rowCdouble.SetFont(Font);

            rowRdouble = workbook.CreateCellStyle();
            rowRdouble.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            rowRdouble.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRdouble.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRdouble.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRdouble.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRdouble.WrapText = true;
            rowRdouble.Alignment = HorizontalAlignment.Right;
            rowRdouble.VerticalAlignment = VerticalAlignment.Center;
            rowRdouble.SetFont(Font);


            //estilos row int
            rowLint = workbook.CreateCellStyle();
            rowLint.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLint.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLint.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLint.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLint.SetFont(Font);
            rowLint.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            rowLint.WrapText = true;
            rowLint.Alignment = HorizontalAlignment.Left;
            rowLint.VerticalAlignment = VerticalAlignment.Center;

            rowCint = workbook.CreateCellStyle();
            rowCint.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCint.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCint.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCint.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCint.SetFont(Font);
            rowCint.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            rowCint.WrapText = true;
            rowCint.Alignment = HorizontalAlignment.Center;
            rowCint.VerticalAlignment = VerticalAlignment.Center;

            rowRint = workbook.CreateCellStyle();
            rowRint.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRint.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRint.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRint.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRint.SetFont(Font);
            rowRint.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            rowRint.WrapText = true;
            rowRint.Alignment = HorizontalAlignment.Right;
            rowRint.VerticalAlignment = VerticalAlignment.Center;


            //estilos row string
            rowLstring = workbook.CreateCellStyle();
            rowLstring.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLstring.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLstring.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLstring.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowLstring.SetFont(Font);
            rowLstring.WrapText = true;
            rowLstring.Alignment = HorizontalAlignment.Left;
            rowLstring.VerticalAlignment = VerticalAlignment.Center;

            rowCstring = workbook.CreateCellStyle();
            rowCstring.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCstring.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCstring.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCstring.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowCstring.SetFont(Font);
            rowCstring.WrapText = true;
            rowCstring.Alignment = HorizontalAlignment.Center;
            rowCstring.VerticalAlignment = VerticalAlignment.Center;

            rowRstring = workbook.CreateCellStyle();
            rowRstring.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRstring.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRstring.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRstring.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            rowRstring.SetFont(Font);
            rowRstring.WrapText = true;
            rowRstring.Alignment = HorizontalAlignment.Right;
            rowRstring.VerticalAlignment = VerticalAlignment.Center;



            //estilos total string
            totalLstring = workbook.CreateCellStyle();
            totalLstring.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLstring.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLstring.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLstring.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLstring.SetFont(FontBold);
            totalLstring.WrapText = true;
            totalLstring.Alignment = HorizontalAlignment.Left;
            totalLstring.VerticalAlignment = VerticalAlignment.Center;

            totalCstring = workbook.CreateCellStyle();
            totalCstring.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCstring.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCstring.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCstring.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCstring.SetFont(FontBold);
            totalCstring.WrapText = true;
            totalCstring.Alignment = HorizontalAlignment.Center;
            totalCstring.VerticalAlignment = VerticalAlignment.Center;

            totalRstring = workbook.CreateCellStyle();
            totalRstring.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRstring.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRstring.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRstring.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRstring.SetFont(FontBold);
            totalRstring.WrapText = true;
            totalRstring.Alignment = HorizontalAlignment.Right;
            totalRstring.VerticalAlignment = VerticalAlignment.Center;

            //estilos total double
            totalLdouble = workbook.CreateCellStyle();
            totalLdouble.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            totalLdouble.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLdouble.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLdouble.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLdouble.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLdouble.WrapText = true;
            totalLdouble.Alignment = HorizontalAlignment.Left;
            totalLdouble.VerticalAlignment = VerticalAlignment.Center;
            totalLdouble.SetFont(FontBold);

            totalCdouble = workbook.CreateCellStyle();
            totalCdouble.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            totalCdouble.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCdouble.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCdouble.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCdouble.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCdouble.WrapText = true;
            totalCdouble.Alignment = HorizontalAlignment.Center;
            totalCdouble.VerticalAlignment = VerticalAlignment.Center;
            totalCdouble.SetFont(FontBold);

            totalRdouble = workbook.CreateCellStyle();
            totalRdouble.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            totalRdouble.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRdouble.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRdouble.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRdouble.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRdouble.WrapText = true;
            totalRdouble.Alignment = HorizontalAlignment.Right;
            totalRdouble.VerticalAlignment = VerticalAlignment.Center;
            totalRdouble.SetFont(FontBold);

            //estilos total int
            totalLint = workbook.CreateCellStyle();
            totalLint.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLint.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLint.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLint.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            totalLint.SetFont(FontBold);
            totalLint.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            totalLint.WrapText = true;
            totalLint.Alignment = HorizontalAlignment.Left;
            totalLint.VerticalAlignment = VerticalAlignment.Center;

            totalCint = workbook.CreateCellStyle();
            totalCint.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCint.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCint.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCint.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            totalCint.SetFont(FontBold);
            totalCint.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            totalCint.WrapText = true;
            totalCint.Alignment = HorizontalAlignment.Center;
            totalCint.VerticalAlignment = VerticalAlignment.Center;

            totalRint = workbook.CreateCellStyle();
            totalRint.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRint.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRint.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRint.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            totalRint.SetFont(FontBold);
            totalRint.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            totalRint.WrapText = true;
            totalRint.Alignment = HorizontalAlignment.Right;
            totalRint.VerticalAlignment = VerticalAlignment.Center;

            #endregion
        }

        /// <summary>
        /// Crea la hoja del libro de excel, agrega el encabezado con titulo, fecha de proceso
        /// y fecha de consulta (estilo: "Fecha de proceso: Viernes 06 de Enero de 2017").
        /// </summary>
        /// <param name="titulo">Título para la hoja del excel y para el título del reporte 
        ///                     (se muestra en mayusculas)</param>
        /// <param name="fechaProceso">Fecha para mostrar como fecha de proceso</param>
        /// <param name="colFinTitulo">Número de columna donde finaliza el merge de celdas
        ///                             para las fechas</param>
        /// <returns>Hoja de Excel</returns>
        public ISheet CreateSheet(string titulo, DateTime fechaProceso, int colFinTitulo)
        {
            var sheetHeader = workbook.CreateSheet(titulo);

            //TITULOS
            var dt = DateTime.Today;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-AR", true);
            var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            string[] newNames = { "Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado" };
            ci.DateTimeFormat.DayNames = newNames;
            string[] newMonths = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre", "" };
            ci.DateTimeFormat.MonthGenitiveNames = newMonths;

            //titulo reporte
            var headerRowHeader0 = sheetHeader.CreateRow(0);
            headerRowHeader0.HeightInPoints = (short)20;
            var tituloReporte = headerRowHeader0.CreateCell(0);
            tituloReporte.SetCellValue(titulo.ToUpper());
            tituloReporte.CellStyle = lCellStyleTituloReporteTitulo;
            var cellRangeTI = new CellRangeAddress(0, 0, 0,4);
            sheetHeader.AddMergedRegion(cellRangeTI);

            //fecha consulta
            
            var fechaConsulta = DateTime.Today.ToString("", ci);
            var fechaReporteConsulta = headerRowHeader0.CreateCell(5);
            fechaReporteConsulta.SetCellValue($"{Res.res.dateofconsultation}: {DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm")}");
            fechaReporteConsulta.CellStyle = lCellStyleTituloReporteFecha;
            var cellRangeTDC = new CellRangeAddress(0, 0, 5, colFinTitulo);
            sheetHeader.AddMergedRegion(cellRangeTDC);

        
            return sheetHeader;
        }

        /// <summary>
        /// Metodo para crear una celda de excel con un tipo y estilo determinado:
        /// </summary>
        /// <param name="Row"> Fila donde se agrega la celda</param>
        /// <param name="index">Indice de la columna de la celda</param>
        /// <param name="value">Valor de la celda</param>
        /// <param name="tipo">Tipo del valor: string, int o double.
        ///                     Se utiliza el enum ExcelTypes.</param>
        /// <param name="estilo">Estilo de la celda: header, row o total
        ///                     , cada uno con el indicador de alineacion: L, C o R (Left, Center o Right).
        ///                      Se utiliza el enum ExcelStyles.
        ///                      ej: headerR  o rowL </param>
        /// <returns>Celda formateada</returns>
        public ICell CreateCell(IRow Row, int index, string value, ExcelTypes tipo, ExcelStyles estilo)
        {
            var celda = Row.CreateCell(index);

            if (!String.IsNullOrEmpty(value))
            {
                switch (tipo)
                {
                    case ExcelTypes.Integer:
                        celda.SetCellValue(Convert.ToInt32(value));
                        break;
                    case ExcelTypes.Text:
                        celda.SetCellValue(value);
                        break;
                    case ExcelTypes.Decimals:
                        celda.SetCellValue(Convert.ToDouble(value));
                        break;
                    default:
                        break;
                }
            }


            switch (estilo)
            {
                //header
                case ExcelStyles.HeaderL:
                    celda.CellStyle = headerL;
                    break;
                case ExcelStyles.HeaderC:
                    celda.CellStyle = headerC;
                    break;
                case ExcelStyles.HeaderR:
                    celda.CellStyle = headerR;
                    break;

                //filas
                case ExcelStyles.RowL:
                    switch (tipo)
                    {
                        case ExcelTypes.Integer:
                            celda.CellStyle = rowLint;
                            break;
                        case ExcelTypes.Text:
                            celda.CellStyle = rowLstring;
                            break;
                        case ExcelTypes.Decimals:
                            celda.CellStyle = rowLdouble;
                            break;
                        default:
                            break;
                    }
                    break;

                case ExcelStyles.RowC:
                    switch (tipo)
                    {
                        case ExcelTypes.Integer:
                            celda.CellStyle = rowCint;
                            break;
                        case ExcelTypes.Text:
                            celda.CellStyle = rowCstring;
                            break;
                        case ExcelTypes.Decimals:
                            celda.CellStyle = rowCdouble;
                            break;
                        default:
                            break;
                    }
                    break;
                case ExcelStyles.RowR:
                    switch (tipo)
                    {
                        case ExcelTypes.Integer:
                            celda.CellStyle = rowRint;
                            break;
                        case ExcelTypes.Text:
                            celda.CellStyle = rowRstring;
                            break;
                        case ExcelTypes.Decimals:
                            celda.CellStyle = rowRdouble;
                            break;
                        default:
                            break;
                    }
                    break;

                //totales
                case ExcelStyles.TotalL:
                    switch (tipo)
                    {
                        case ExcelTypes.Integer:
                            celda.CellStyle = totalLint;
                            break;
                        case ExcelTypes.Text:
                            celda.CellStyle = totalLstring;
                            break;
                        case ExcelTypes.Decimals:
                            celda.CellStyle = totalLdouble;
                            break;
                        default:
                            break;
                    }
                    break;
                case ExcelStyles.TotalC:
                    switch (tipo)
                    {
                        case ExcelTypes.Integer:
                            celda.CellStyle = totalCint;
                            break;
                        case ExcelTypes.Text:
                            celda.CellStyle = totalCstring;
                            break;
                        case ExcelTypes.Decimals:
                            celda.CellStyle = totalCdouble;
                            break;
                        default:
                            break;
                    }
                    break;
                case ExcelStyles.TotalR:
                    switch (tipo)
                    {
                        case ExcelTypes.Integer:
                            celda.CellStyle = totalRint;
                            break;
                        case ExcelTypes.Text:
                            celda.CellStyle = totalRstring;
                            break;
                        case ExcelTypes.Decimals:
                            celda.CellStyle = totalRdouble;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return celda;
        }

    }

    public enum ExcelStyles
    {
        HeaderL, HeaderC, HeaderR
        , RowL, RowC, RowR
        , TotalL, TotalC, TotalR
    };

    public enum ExcelTypes { Text, Integer, Decimals };

}
