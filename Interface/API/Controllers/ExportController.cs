using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using AI.V2.BaseLibrary;
using MigraDoc.Rendering;
using DocumentFormat.OpenXml;
using System.Linq;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Drawing;

namespace AI.V2.Controllers
{
    /// <summary>
    /// for returning spreadsheets in csv, xls or pdf form
    /// </summary>
    public class ExportController : ApiController
    {
        /// <summary>
        /// returns a pdf, xlsx, or csv file
        /// with only the count of the objects by object class
        /// </summary>
        /// <param name="objs">the objects to count</param>
        /// <param name="FormatString">pdf, xlsx or csv</param>
        /// <returns>pdf, xlsx or csv of count of objects by class</returns>
        [HttpPost, Route("ExportCategorical/{FormatString}")]
        public IHttpActionResult ExportCategorical([FromBody] List<BaseLibrary.AIObject> objs, string FormatString)
        {


            /*start debug string*/
            //objs = new List<AIObject>();
            //objs.Add(new AIObject { ObjectClass = "thing" });
            //objs.Add(new AIObject { ObjectClass = "thing" });
            //objs.Add(new AIObject { ObjectClass = "thing" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing3" });
            //objs.Add(new AIObject { ObjectClass = "thing3" });
            /*end debug string*/
            AcceptedFormats Format;
            if (!AI.V2.BaseLibrary.AcceptedFormats.TryParse(FormatString, true, out Format))
            {
                var errors = new List<string>();
                errors.Add("Cannot currently export data in " + FormatString + " file type");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errors[0]); //debug
                throw new ArgumentException(errors[0]); //debug
                //return View("~/Views/Shared/CustomError.cshtml", errors);
            }
            string FileName = "Objects"; //debug
            //String FormatString = "xlsx"; //debug
            var resp = HttpContext.Current.Response;
            resp.Clear();
            resp.ClearHeaders();
            resp.ClearContent();
            resp.AddHeader("Pragma", "no-cache");
            writeOutputFile(FileName, objs, resp, Format, true);
            return Ok();
        }

        /// <summary>
        /// Returns a pdf, xlsx or csv from an object
        /// </summary>
        /// <param name="objs">Object that will be turned into csv</param>
        /// <param name="FileName">the filename for the csv</param>
        /// <returns>A xlsx or csv file with headers as property names and rows as property values
        /// in semicolon-separated form or a pdf listing each object individually</returns>
        [HttpPost, Route("Export/{FormatString?}")] 
        public IHttpActionResult ExportData([FromBody] List<BaseLibrary.AIObject> objs, string FormatString = "xlsx")
        {
            /*start debug string*/
            //objs = new List<AIObject>();
            //objs.Add(new AIObject { ObjectClass = "thing", Meta = new Meta { Properties = new List<Property> } });
            //objs.Add(new AIObject { ObjectClass = "thing" });
            //objs.Add(new AIObject { ObjectClass = "thing" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing2" });
            //objs.Add(new AIObject { ObjectClass = "thing3" });
            //objs.Add(new AIObject { ObjectClass = "thing3" });
            /*end debug string*/

            Security.AssertRole(Request.Headers, Security.Role.User);
            AcceptedFormats Format;
            if (!AI.V2.BaseLibrary.AcceptedFormats.TryParse(FormatString, true, out Format))
            {
                var errors = new List<string>();
                errors.Add("Cannot currently export data in " + FormatString + " file type");
                throw new ArgumentException(errors[0]);
                //return RedirectToRoute("~/Views/Shared/CustomError.cshtml", errors);
            }

            string FileName = "Objects"; //debug
            //String FormatString = "xlsx"; //debug
            var resp = HttpContext.Current.Response;
            resp.Clear();
            resp.ClearHeaders();
            resp.ClearContent();
            resp.AddHeader("Pragma", "no-cache");

            writeOutputFile(FileName, objs, resp, Format, false);

            //resp.Flush();
            //resp.Close();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            return Ok();
        }

        /// <summary>
        /// Writes to appropriate output file
        /// </summary>
        /// <param name="FileName">Name of file</param>
        /// <param name="headers">List of strings headers</param>
        /// <param name="rows">List of list of strings rows></param>
        /// <param name="resp">response stream</param>
        /// <param name="Format">AcceptedFormats enum of file format</param>
        /// <param name="Categorical">If true just returns count of categories in a table</param>
        private void writeOutputFile(string FileName, List<AI.V2.BaseLibrary.AIObject> objs, HttpResponse resp, AcceptedFormats Format, bool Categorical)
        {
            switch (Format)
            {
                case AcceptedFormats.CSV:
                    writeCsvToResponse(FileName, objs, resp, Categorical);
                    break;
                case AcceptedFormats.XLSX:
                    writeXlsxToResponse(FileName, objs, resp, Categorical);
                    break;
                case AcceptedFormats.PDF:
                    writePdfToResponse(FileName, objs, resp, Categorical);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unimplemented format enumerator.");
            }
        }

        /// <summary>
        /// writes data as a pdf file
        /// </summary>
        /// <param name="FileName">name of file</param>
        /// <param name="objs">objects to write</param>
        /// <param name="resp">HttpResponse being used</param>
        /// <param name="Categorical">If true just returns count of categories in a table</param>
        private void writePdfToResponse(string FileName, List<AIObject> objs, HttpResponse resp, bool Categorical)
        {
            using (MemoryStream mstr = new MemoryStream())
            {
                string attachment = "attachment; fileneame" + FileName + ".pdf";

                Document doc = styleDocument(FileName);

                if (Categorical)
                {
                    var categoryCount = CountCategoricalData(objs);
                    foreach(var k in categoryCount.Keys)
                    {
                        doc.LastSection.AddParagraph("Class: " + k, "Heading2");
                        doc.LastSection.AddParagraph("Count: " + categoryCount[k]);
                    }
                }
                else
                {
                    foreach (var o in objs)
                    {
                        doc.LastSection.AddParagraph("Class: " + o.ObjectClass, "Heading1");
                        foreach (var p in o.Meta.Properties)
                        {
                            doc.LastSection.AddParagraph("Property:" + p.PropertyClass, "Heading2");
                            var para = doc.LastSection.AddParagraph("", "Normal");
                            for (int i = 0; i < p.Value.Count; i++)
                            {
                                para.AddText(p.Value[i]);
                                if (i < p.Value.Count - 1)
                                    para.AddText(", ");
                            }
                            doc.LastSection.AddParagraph("");
                        }
                    }
                }
                resp.AddHeader("content-disposition", attachment);
                resp.ContentType = "applications/pdf";
                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
                renderer.Document = doc;
                renderer.RenderDocument();
                renderer.PdfDocument.Save(mstr, false);


                resp.BinaryWrite(mstr.ToArray());

            }

        }

        /// <summary>
        /// Writes data as Excel sheet to response
        /// </summary>
        /// <param name="FileName">Name of file</param>
        /// <param name="headers">List of strings headers</param>
        /// <param name="rows">List of list of strings rows></param>
        /// <param name="resp">response stream</param>
        /// <param name="Categorical">If true just returns count of categories in a table</param>
        private void writeXlsxToResponse(string FileName, List<BaseLibrary.AIObject> objs, HttpResponse resp, bool Categorical)
        {
            string attachment = "attachment; fileneame" + FileName + ".xlsx";

            /*start debug section*
            resp.Write("Size of objects is " + objs.Count);
            resp.AddHeader("content-disposition", attachment);
            resp.ContentType = "applications/ms-excel";
            /*end debug section*/


            using (MemoryStream mstr = new MemoryStream())
            {
                using (SpreadsheetDocument spreadSheetDoc = SpreadsheetDocument.Create(mstr, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = spreadSheetDoc.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    WorkbookStylesPart stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = GenerateStyleSheet();
                    stylesPart.Stylesheet.Save();




                    Sheets sheets = spreadSheetDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                    Sheet sheet = new Sheet()
                    {
                        Id = spreadSheetDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Categories"
                    };
                    sheets.Append(sheet);
                    //Categorical = false; //debug
                    if (Categorical)
                    {
                        var categoryCount = CountCategoricalData(objs);
                        string[] headers = { "Category", "Count" };
                        Row headerRow = new Row() { RowIndex = 1 };
                        for (int h = 0; h < headers.Length; h++)
                        {
                            var cr = getCellRef(h, 0);
                            var hdr = headers[h];
                            Cell headerCell = new Cell() { CellReference = cr, InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(hdr) }, DataType = CellValues.InlineString, StyleIndex = 1 };
                            headerRow.InsertAt(headerCell, h);
                        }
                        sheetData.Append(headerRow);
                        UInt32 i = 0;
                        foreach (var k in categoryCount.Keys)
                        {
                            // row index + 1 because Excel starts counting rows at 1 and + 1 more because header is row 1
                            Row r = new Row() { RowIndex = (DocumentFormat.OpenXml.UInt32Value)(i + 2) };
                            Cell cat = new Cell() { CellReference = getCellRef(0, i + 1), InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(k) }, DataType = CellValues.InlineString, StyleIndex = 0 };
                            r.InsertAt(cat, 0);
                            Cell count = new Cell() { CellReference = getCellRef(1, i + 1), CellValue = new CellValue(categoryCount[k] + ""), DataType = CellValues.Number, StyleIndex = 0 };
                            r.InsertAt(count, 1);

                            sheetData.Append(r);
                            i++;
                        }
                        InsertChartInSpreadsheet(spreadSheetDoc, "Categorical Data", categoryCount);
                    }
                    else
                    {
                        List<string> headers = new List<string>();
                        List<List<string>> rows = new List<List<string>>();
                        MakeHeadersAndRows(objs, out headers, out rows);

                        Row headerRow = new Row() { RowIndex = 1 };
                        for (int i = 0; i < headers.Count; i++)
                        {
                            var cr = getCellRef(i, 0);
                            var hdr = headers[i];
                            Cell headerCell = new Cell() { CellReference = cr, InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(hdr) }, DataType = CellValues.InlineString, StyleIndex = 1 };
                            headerRow.InsertAt(headerCell, i);
                        }
                        sheetData.Append(headerRow);
                        for (UInt32 i = 0; i < rows.Count; i++)
                        {
                            // row index + 1 because Excel starts counting rows at 1 and + 1 more because header is row 1
                            Row r = new Row() { RowIndex = (DocumentFormat.OpenXml.UInt32Value)(i + 2) };
                            for (int j = 0; j < rows[(int)i].Count; j++)
                            {
                                Cell c = new Cell() { CellReference = getCellRef(j, i + 1), InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(rows[(int)i][j]) }, DataType = CellValues.InlineString, StyleIndex = 0 };
                                r.InsertAt(c, j);
                            }
                            sheetData.Append(r);
                        }
                    }
                }
                resp.AddHeader("content-disposition", attachment);
                resp.ContentType = "applications/ms-excel";

                resp.BinaryWrite(mstr.ToArray());

            }

        }

        /// <summary>
        /// Writes response to a csv file
        /// </summary>
        /// <param name="FileName">name of file</param>
        /// <param name="headers">list of headers</param>
        /// <param name="rows">list of list of strings to make rows</param>
        /// <param name="csv">StringBuilder to write</param>
        /// <param name="resp">response stream</param>
        /// <param name="Categorical">If true just returns count of categories in a table</param>
        private void writeCsvToResponse(string FileName, List<BaseLibrary.AIObject> objs, HttpResponse resp, bool Categorical)
        {


            StringBuilder csv = new StringBuilder();

            string attachment = "attachment; filename=" + FileName + ".csv";
            resp.AddHeader("content-disposition", attachment);
            resp.ContentType = "text/csv";
            if (Categorical)
            {
                var categoryCount = CountCategoricalData(objs);
                csv.AppendLine("Category,Count");
                foreach(var category in categoryCount.Keys)
                {
                    var row = "";
                    row += category;
                    row += ",";
                    row += categoryCount[category];
                    csv.AppendLine(row);
                }
                resp.Write(csv);
            }
            else
            {
                List<string> headers = new List<string>();
                List<List<string>> rows = new List<List<string>>();

                MakeHeadersAndRows(objs, out headers, out rows);

                csv.AppendLine(TransformDataLineIntoCsv(headers));
                foreach (var r in rows)
                    csv.AppendLine(TransformDataLineIntoCsv(r));
                resp.Write(csv);
            }
        }

        /// <summary>
        /// Turns a row and column number into a Excel Letter/Number cell
        /// reference string, i.e. column=3, row = 2 -> "C3"
        /// </summary>
        /// <param name="columnNumber">The column number</param>
        /// <param name="rowNumber">The row number</param>
        /// <returns>String cell reference</returns>
        private string getCellRef(int columnNumber, UInt32 rowNumber)
        {
            int maxCol = 16384;
            int maxRow = 1048576;
            if (columnNumber > maxCol)
            {
                throw new ArgumentOutOfRangeException("columnNumber", columnNumber, "Too many columns for Excel sheet");
            }
            string cr = "";
            int[] letterVals = new int[3];
            letterVals[0] = (int)Math.Floor((double)columnNumber / 676);
            letterVals[1] = (int)Math.Floor((double)(columnNumber % 676) / 26);
            letterVals[2] = columnNumber % 26;
            for (int i = 0; i < 2; i++)
            {
                if (letterVals[i] > 0)
                {
                    char prevLetter = (char)(65 + letterVals[0]);
                    cr += prevLetter;
                }
            }
            char mainLetter = (char)(65 + letterVals[2]);
            cr += mainLetter;
            if (rowNumber > maxRow)
            {
                throw new ArgumentOutOfRangeException("rowNumber", rowNumber, "Too many rows for Excel sheet");
            }
            rowNumber++; //because Excel cells start from 1
            cr += rowNumber;
            return cr;
        }

        /// <summary>
        /// Turns a list string into csv row
        /// </summary>
        /// <param name="l">string to turn to csv</param>
        /// <returns>a row of every item in l separated by commas</returns>
        /// <remarks>does not expand csv column to match other columns</remarks>
        private string TransformDataLineIntoCsv(List<string> l)
        {
            var row = "";
            for (int i = 0; i < l.Count; i++)
            {
                string s = l[i];
                row += '"' + s + '"';
                if (i < l.Count - 1)
                    row += ',';
            }
            return row;
        }

        /// <summary>
        /// Creates a row of empty strings
        /// </summary>
        /// <param name="l">length of the row</param>
        /// <returns>["",""...,""] where length = l</returns>
        private List<string> emptyRow(int l)
        {
            var r = new List<string>();
            for (int i = 0; i < l; i++)
                r.Add("");
            return r;
        }

        /// <summary>
        /// adds one item to each list in a list of list of strings
        /// </summary>
        /// <param name="rows">the orighinal list</param>
        /// <returns>the list with one more string in each list</returns>
        private List<List<string>> addColumn(List<List<string>> rows)
        {
            foreach (var prevR in rows)
                prevR.Add("");
            return rows;
        }

        /// <summary>
        /// Returns a cell value of either single string value or
        /// semicolon-separated values
        /// </summary>
        /// <param name="p">A Property with values</param>
        /// <returns>A string with a single value and no semicolon</returns>
        private string setCellVal(AI.V2.BaseLibrary.Property p)
        {
            string values = "";
            if (p.Value.Count == 1)
            {
                values = p.Value[0];
            }
            else
            {
                foreach (var v in p.Value)
                {
                    values += v.ToString() + ";";
                }
            }
            return values;
        }

        /// <summary>
        /// Makes the headers and rows for csv and excel files
        /// </summary>
        /// <param name="objs">the ai objects to make the headers from</param>
        /// <param name="headers">out file for headers</param>
        /// <param name="rows">out file for rows</param>
        /// <remarks>headers and rows probably don't have to be out but I do it that way so I know they're changed in the method</remarks>
        private void MakeHeadersAndRows(List<BaseLibrary.AIObject> objs, out List<string> headers, out List<List<string>> rows)
        {
            headers = new List<string>();
            rows = new List<List<string>>();

            //if (objs == null) //debug
            //    throw new NullReferenceException("objs is null"); // debug

            foreach (var o in objs)
            {

                foreach (var p in o.Meta.Properties)
                {

                    var r = emptyRow(headers.Count);
                    var values = setCellVal(p);
                    var ind = headers.IndexOf(p.PropertyClass);
                    if (ind != -1)
                    {
                        // if the header exists add value at that index of row
                        r[ind] = values;
                    }
                    else
                    {
                        // if not then add the header, create a new column
                        // in previous rows and add value to end of row
                        headers.Add(p.PropertyClass);
                        rows = addColumn(rows);
                        r.Add(values);
                    }
                    rows.Add(r);
                }

            }

        }

        /// <summary>
        /// Creates the pdf document and sets its styles
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>name of file</returns>
        private Document styleDocument(string FileName)
        {
            Document doc = new Document();
            Section sec = doc.AddSection();
            var title = doc.LastSection.AddParagraph();
            title.Format.Font.Size = 18;
            title.Format.Font.Bold = true;
            title.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Blue;
            title.AddText(FileName);

            MigraDoc.DocumentObjectModel.Style heading1 = doc.Styles["Heading1"];
            heading1.Font.Size = 16;
            heading1.Font.Bold = true;
            heading1.Font.Color = MigraDoc.DocumentObjectModel.Colors.Blue;

            MigraDoc.DocumentObjectModel.Style heading2 = doc.Styles["Heading2"];
            heading1.Font.Size = 14;
            heading1.Font.Bold = true;
            heading1.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            MigraDoc.DocumentObjectModel.Style normal = doc.Styles["Normal"];
            normal.Font.Size = 12;
            normal.Font.Bold = false;
            normal.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
            return doc;
        }

        /// <summary>
        /// counts the number of objects in each object class
        /// and returns as dictionary
        /// </summary>
        /// <param name="objs">The objects to count</param>
        /// <returns></returns>
        private Dictionary<string, int> CountCategoricalData(List<BaseLibrary.AIObject> objs)
        {
            var categoryCount = new Dictionary<string, int>();
            foreach(var obj in objs)
            {
                var cls = obj.ObjectClass;
                if (categoryCount.ContainsKey(cls))
                {
                    categoryCount[cls]++;
                } else
                {
                    categoryCount.Add(cls, 1);
                }
            }
            return categoryCount;
            
        }

        private Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(


                new DocumentFormat.OpenXml.Spreadsheet.Fonts(
                    new DocumentFormat.OpenXml.Spreadsheet.Font(                                                               // Index 0 - The default font.
                        new FontSize() { Val = 11 },
                        new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new DocumentFormat.OpenXml.Spreadsheet.Font(                                                               // Index 1 - The bold font.
                        new Bold(),
                        new FontSize() { Val = 11 },
                        new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "FFFFFF" } },
                        new FontName() { Val = "Calibri" }),
                    new DocumentFormat.OpenXml.Spreadsheet.Font(                                                               // Index 2 - The Italic font.
                        new Italic(),
                        new FontSize() { Val = 11 },
                        new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new DocumentFormat.OpenXml.Spreadsheet.Font(                                                               // Index 3 - The Times Roman font. with 16 size
                        new FontSize() { Val = 16 },
                        new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Times New Roman" })
                ),
                new Fills(
                    new DocumentFormat.OpenXml.Spreadsheet.Fill(                                                           // Index 0 - The default fill.
                        new DocumentFormat.OpenXml.Spreadsheet.PatternFill() { PatternType = PatternValues.None }),
                    new DocumentFormat.OpenXml.Spreadsheet.Fill(                                                           // Index 0 - The default fill.
                        new DocumentFormat.OpenXml.Spreadsheet.PatternFill() { PatternType = PatternValues.None }),
                    new DocumentFormat.OpenXml.Spreadsheet.Fill(                                                           // Index 2 - The gray fill.
                        new DocumentFormat.OpenXml.Spreadsheet.PatternFill(
                            new DocumentFormat.OpenXml.Spreadsheet.ForegroundColor() { Rgb = new HexBinaryValue() { Value = "0000FF" } }
                        )
                        { PatternType = PatternValues.Solid })
                ),
                new DocumentFormat.OpenXml.Spreadsheet.Borders(
                    new DocumentFormat.OpenXml.Spreadsheet.Border(                                                         // Index 0 - The default border.
                        new DocumentFormat.OpenXml.Spreadsheet.LeftBorder(),
                        new DocumentFormat.OpenXml.Spreadsheet.RightBorder(),
                        new DocumentFormat.OpenXml.Spreadsheet.TopBorder(),
                        new DocumentFormat.OpenXml.Spreadsheet.BottomBorder(),
                        new DiagonalBorder()),
                    new DocumentFormat.OpenXml.Spreadsheet.Border(                                                         // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                        new DocumentFormat.OpenXml.Spreadsheet.TopBorder(
                            new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.BottomBorder(
                            new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thick },
                        new DiagonalBorder()
                    )
                ),
                new CellFormats(
                //new CellFormat() { FontId = 1, FillId = 1, BorderId = 0 }//debug



                new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },                          // Index 0 - The default cell style.  If a cell does not have a style index applied it will use this style combination instead
                new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 1, FillId = 2, BorderId = 0, ApplyFont = true }
                )
            ); // return
        }

        private static void InsertChartInSpreadsheet(SpreadsheetDocument document, string title,
Dictionary<string, int> data)
        {
            WorksheetPart graphWorksheetPart = (WorksheetPart)document.WorkbookPart.AddNewPart<WorksheetPart>();
            graphWorksheetPart.Worksheet = new Worksheet(new SheetData());
            Sheets sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>() ;

            Sheet sheet = new Sheet()
            {
                Id = document.WorkbookPart.GetIdOfPart(graphWorksheetPart),
                SheetId = 2,
                Name = "Graph"
            };
            sheets.Append(sheet);

            // Add a new drawing to the worksheet.
            DrawingsPart drawingsPart = graphWorksheetPart.AddNewPart<DrawingsPart>();
            graphWorksheetPart.Worksheet.Append(new DocumentFormat.OpenXml.Spreadsheet.Drawing()
            { Id = graphWorksheetPart.GetIdOfPart(drawingsPart) });
            graphWorksheetPart.Worksheet.Save();

            // Add a new chart and set the chart language to English-US.
            ChartPart chartPart = drawingsPart.AddNewPart<ChartPart>();
            chartPart.ChartSpace = new ChartSpace();
            chartPart.ChartSpace.Append(new EditingLanguage() { Val = new StringValue("en-US") });
            DocumentFormat.OpenXml.Drawing.Charts.Chart chart = chartPart.ChartSpace.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.Chart>(
                new DocumentFormat.OpenXml.Drawing.Charts.Chart());

            // Create a new clustered column chart.
            PlotArea plotArea = chart.AppendChild<PlotArea>(new PlotArea());
            Layout layout = plotArea.AppendChild<Layout>(new Layout());
            BarChart barChart = plotArea.AppendChild<BarChart>(new BarChart(new BarDirection()
            { Val = new EnumValue<BarDirectionValues>(BarDirectionValues.Column) },
                new BarGrouping() { Val = new EnumValue<BarGroupingValues>(BarGroupingValues.Clustered) }));

            uint i = 0;

            // Iterate through each key in the Dictionary collection and add the key to the chart Series
            // and add the corresponding value to the chart Values.
            foreach (string key in data.Keys)
            {
                BarChartSeries barChartSeries = barChart.AppendChild<BarChartSeries>(new BarChartSeries(new Index()
                {
                    Val =
     new UInt32Value(i)
                },
                    new Order() { Val = new UInt32Value(i) },
                    new SeriesText(new NumericValue() { Text = key })));

                StringLiteral strLit = barChartSeries.AppendChild<CategoryAxisData>(new CategoryAxisData()).AppendChild<StringLiteral>(new StringLiteral());
                strLit.Append(new PointCount() { Val = new UInt32Value(1U) });
                strLit.AppendChild<StringPoint>(new StringPoint() { Index = new UInt32Value(0U) }).Append(new NumericValue(title));

                NumberLiteral numLit = barChartSeries.AppendChild<DocumentFormat.OpenXml.Drawing.Charts.Values>(
                    new DocumentFormat.OpenXml.Drawing.Charts.Values()).AppendChild<NumberLiteral>(new NumberLiteral());
                numLit.Append(new FormatCode("General"));
                numLit.Append(new PointCount() { Val = new UInt32Value(1U) });
                numLit.AppendChild<NumericPoint>(new NumericPoint() { Index = new UInt32Value(0u) }).Append
    (new NumericValue(data[key].ToString()));

                i++;
            }

            barChart.Append(new AxisId() { Val = new UInt32Value(48650112u) });
            barChart.Append(new AxisId() { Val = new UInt32Value(48672768u) });

            // Add the Category Axis.
            CategoryAxis catAx = plotArea.AppendChild<CategoryAxis>(new CategoryAxis(new AxisId()
            { Val = new UInt32Value(48650112u) }, new Scaling(new DocumentFormat.OpenXml.Drawing.Charts.Orientation()
            {
                Val = new EnumValue<DocumentFormat.
    OpenXml.Drawing.Charts.OrientationValues>(DocumentFormat.OpenXml.Drawing.Charts.OrientationValues.MinMax)
            }),
                new AxisPosition() { Val = new EnumValue<AxisPositionValues>(AxisPositionValues.Bottom) },
                new TickLabelPosition() { Val = new EnumValue<TickLabelPositionValues>(TickLabelPositionValues.NextTo) },
                new CrossingAxis() { Val = new UInt32Value(48672768U) },
                new Crosses() { Val = new EnumValue<CrossesValues>(CrossesValues.AutoZero) },
                new AutoLabeled() { Val = new BooleanValue(true) },
                new LabelAlignment() { Val = new EnumValue<LabelAlignmentValues>(LabelAlignmentValues.Center) },
                new LabelOffset() { Val = new UInt16Value((ushort)100) }));

            // Add the Value Axis.
            ValueAxis valAx = plotArea.AppendChild<ValueAxis>(new ValueAxis(new AxisId() { Val = new UInt32Value(48672768u) },
                new Scaling(new DocumentFormat.OpenXml.Drawing.Charts.Orientation()
                {
                    Val = new EnumValue<DocumentFormat.OpenXml.Drawing.Charts.OrientationValues>(
                    DocumentFormat.OpenXml.Drawing.Charts.OrientationValues.MinMax)
                }),
                new AxisPosition() { Val = new EnumValue<AxisPositionValues>(AxisPositionValues.Left) },
                new MajorGridlines(),
                new DocumentFormat.OpenXml.Drawing.Charts.NumberingFormat()
                {
                    FormatCode = new StringValue("General"),
                    SourceLinked = new BooleanValue(true)
                }, new TickLabelPosition()
                {
                    Val = new EnumValue<TickLabelPositionValues>
    (TickLabelPositionValues.NextTo)
                }, new CrossingAxis() { Val = new UInt32Value(48650112U) },
                new Crosses() { Val = new EnumValue<CrossesValues>(CrossesValues.AutoZero) },
                new CrossBetween() { Val = new EnumValue<CrossBetweenValues>(CrossBetweenValues.Between) })

                );

            // Add the chart Legend.
            Legend legend = chart.AppendChild<Legend>(new Legend(new LegendPosition() { Val = new EnumValue<LegendPositionValues>(LegendPositionValues.Right) },
                new Layout()));

            chart.Append(new PlotVisibleOnly() { Val = new BooleanValue(true) });

            // Save the chart part.
            chartPart.ChartSpace.Save();

            // Position the chart on the worksheet using a TwoCellAnchor object.
            drawingsPart.WorksheetDrawing = new WorksheetDrawing();
            TwoCellAnchor twoCellAnchor = drawingsPart.WorksheetDrawing.AppendChild<TwoCellAnchor>(new TwoCellAnchor());
            twoCellAnchor.Append(new DocumentFormat.OpenXml.Drawing.Spreadsheet.FromMarker(new ColumnId("1"),
                new ColumnOffset("581025"),
                new RowId("1"),
                new RowOffset("114300")));
            twoCellAnchor.Append(new DocumentFormat.OpenXml.Drawing.Spreadsheet.ToMarker(new ColumnId("9"),
                new ColumnOffset("276225"),
                new RowId("16"),
                new RowOffset("0")));

            // Append a GraphicFrame to the TwoCellAnchor object.
            DocumentFormat.OpenXml.Drawing.Spreadsheet.GraphicFrame graphicFrame =
                twoCellAnchor.AppendChild<DocumentFormat.OpenXml.
    Drawing.Spreadsheet.GraphicFrame>(new DocumentFormat.OpenXml.Drawing.
    Spreadsheet.GraphicFrame());
            graphicFrame.Macro = "";

            graphicFrame.Append(new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualGraphicFrameProperties(
                new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties() { Id = new UInt32Value(2u), Name = "Chart 1" },
                new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualGraphicFrameDrawingProperties()));

            graphicFrame.Append(new Transform(new Offset() { X = 0L, Y = 0L },
                                                                    new Extents() { Cx = 0L, Cy = 0L }));

            graphicFrame.Append(new Graphic(new GraphicData(new ChartReference() { Id = drawingsPart.GetIdOfPart(chartPart) })
            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" }));

            twoCellAnchor.Append(new ClientData());

            // Save the WorksheetDrawing object.
            drawingsPart.WorksheetDrawing.Save();


        }

    }

}

