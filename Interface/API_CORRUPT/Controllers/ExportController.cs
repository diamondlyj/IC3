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

namespace AI.V2.Controllers
{
    /// <summary>
    /// for returning spreadsheets in csv or xls form
    /// </summary>
    public class ExportController : ApiController
    {
        public object AcceptedFormats { get; private set; }

        /// <summary>
        /// Returns a csv from an object
        /// </summary>
        /// <param name="objs">Object that will be turned into csv</param>
        /// <param name="FileName">the filename for the csv</param>
        /// <returns>A csv file with headers as property names and rows as property values
        /// in semicolon-separated form</returns>
        [HttpPost Route("Export/{FormatString?}")] //debug
        public IHttpActionResult ExportSpreadsheet([FromBody] List<BaseLibrary.AIObject> objs, string FormatString = "xlsx")
        {
            Security.AssertRole(Request.Headers);
            AcceptedFormats Format;
            if (!AI.V2.BaseLibrary.AcceptedFormats.TryParse(FormatString, true, out Format))
            {
                var errors = new List<string>();
                errors.Add("Cannot currently export data in " + FormatString + " file type");
                throw new ArgumentException(errors[0]);
                //return RedirectToRoute("~/Views/Shared/CustomError.cshtml", errors);
            }

            //[tk] use downloadformatinfo dictionary like on other controller
            string FileName = "Objects"; //debug
            //String FormatString = "xlsx"; //debug
            var resp = HttpContext.Current.Response;
            resp.Clear();
            resp.ClearHeaders();
            resp.ClearContent();
            resp.AddHeader("Pragma", "no-cache");

            writeOutputFile(FileName, objs, resp, Format);

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
        private void writeOutputFile(string FileName, List<AI.V2.BaseLibrary.AIObject> objs, HttpResponse resp, AcceptedFormats Format)
        {
            List<string> headers = new List<string>();
            List<List<string>> rows = new List<List<string>>();
            switch (Format)
            {
                case AcceptedFormats.CSV:
                    MakeHeadersAndRows(objs, out headers, out rows);
                    writeCsvToResponse(FileName, headers, rows, resp);
                    break;
                case AcceptedFormats.XLSX:
                    MakeHeadersAndRows(objs, out headers, out rows);
                    writeXlsxToResponse(FileName, headers, rows, resp);
                    break;
                case AcceptedFormats.PDF:
                    writePdfToResponse(FileName, objs, resp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unimplemented format enumerator");
            }
        }


        private void writePdfToResponse(string FileName, List<AIObject> objs, HttpResponse resp)
        {
            using (MemoryStream mstr = new MemoryStream())
            {
                string attachment = "attachment; fileneame" + FileName + ".pdf";

                Document doc = new Document();
                Section sec = doc.AddSection();
                var title = doc.LastSection.AddParagraph();
                title.Format.Font.Size = 18;
                title.Format.Font.Bold = true;
                title.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Blue;
                title.AddText(FileName);

                Style heading1 = doc.Styles["Heading1"];
                heading1.Font.Size = 16;
                heading1.Font.Bold = true;
                heading1.Font.Color = MigraDoc.DocumentObjectModel.Colors.Blue;

                Style heading2 = doc.Styles["Heading2"];
                heading1.Font.Size = 14;
                heading1.Font.Bold = true;
                heading1.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                Style normal = doc.Styles["Normal"];
                normal.Font.Size = 12;
                normal.Font.Bold = false;
                normal.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                foreach(var o in objs)
                {
                    doc.LastSection.AddParagraph("Class" + o.ObjectClass, "Heading1");
                    foreach(var p in o.Meta.Properties)
                    {
                        doc.LastSection.AddParagraph("Property:" + p.PropertyClass, "Heading2");
                        var para = doc.LastSection.AddParagraph("", "Normal");
                        for (int i = 0; i < p.Value.Count; i++) {
                            para.AddText(p.Value[i]);
                            if (i < p.Value.Count - 1)
                                para.AddText(", ");
                        }
                        doc.LastSection.AddParagraph("");
                    }
                }
                resp.AddHeader("content-disposition", attachment);
                resp.ContentType = "applications/pdf";

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
        private void writeXlsxToResponse(string FileName, List<string> headers, List<List<string>> rows, HttpResponse resp)
        {
            string attachment = "attachment; fileneame" + FileName + ".xlsx";
            using (MemoryStream mstr = new MemoryStream())
            {
                using (SpreadsheetDocument spreadSheetDoc = SpreadsheetDocument.Create(mstr, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = spreadSheetDoc.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = spreadSheetDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                    Sheet sheet = new Sheet()
                    {
                        Id = spreadSheetDoc.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Sheet1"
                    };


                    sheets.Append(sheet);
                    Row headerRow = new Row() { RowIndex = 1 };
                    for (int i = 0; i < headers.Count; i++)
                    {
                        var cr = getCellRef(i, 0);
                        var hdr = headers[i];
                        Cell headerCell = new Cell() { CellReference = cr, CellValue = new CellValue(hdr), DataType = CellValues.String };
                        headerRow.InsertAt(headerCell, i);
                    }
                    sheetData.Append(headerRow);
                    for (UInt32 i = 0; i < rows.Count; i++)
                    {
                        // row index + 1 because Excel starts counting rows at 1 and + 1 more because header is row 1
                        Row r = new Row() { RowIndex = (DocumentFormat.OpenXml.UInt32Value)(i + 2) };
                        for (int j = 0; j < rows[(int)i].Count; j++)
                        {
                            Cell c = new Cell() { CellReference = getCellRef(j, i + 1), CellValue = new CellValue(rows[(int)i][j]), DataType = CellValues.String };
                            r.InsertAt(c, j);
                        }
                        sheetData.Append(r);
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
        private void writeCsvToResponse(string FileName, List<string> headers, List<List<string>> rows,  HttpResponse resp)
        {
            StringBuilder csv = new StringBuilder();

            string attachment = "attachment; filename=" + FileName + ".csv";
            resp.AddHeader("content-disposition", attachment);
            resp.ContentType = "text/csv";
            csv.AppendLine(TransformDataLineIntoCsv(headers));
            foreach (var r in rows)
                csv.AppendLine(TransformDataLineIntoCsv(r));
            resp.Write(csv);

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
            letterVals[1] = (int)Math.Floor((double)(columnNumber%676) / 26);
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
            if(rowNumber > maxRow)
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
            for(int i=0;i<l.Count;i++)
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
        private List<string> emptyRow (int l)
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
    }

}
