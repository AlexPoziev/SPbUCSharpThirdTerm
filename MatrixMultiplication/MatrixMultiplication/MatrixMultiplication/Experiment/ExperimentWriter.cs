using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using IronSoftware.Drawing;
using PdfDocument = iText.Kernel.Pdf.PdfDocument;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace MatrixMultiplication.Experiment;

/// <summary>
/// Class that perform report of the experiment.
/// </summary>
public static class ExperimentWriter
{
    /// <summary>
    /// Method that creates a table of experiment result in .pdf file.
    /// </summary>
    public static void CreateTable(
        ((int rowSize, int columnSize) firstMatrixSize, (int rowSize, int columnSize) secondMatrixSize)[] matricesSizes)
    {
        ArgumentNullException.ThrowIfNull(matricesSizes);

        var writer = new PdfWriter("./test.pdf");
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        var intervalInfo = new Paragraph("Confidence interval = 95%")
            .SetFontSize(10);

        document.Add(intervalInfo);

        var table = new Table(3, false);

        var cells = new List<Cell>
        {
            new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph("Matrices sizes")),
            new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph("Sequental")),
            new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph("Parallel"))
        };

        foreach (var size in matricesSizes)
        {
            var parallelResult =
                ExperimentEvaluations.EvaluateMatrixMultiplication(size.firstMatrixSize, size.secondMatrixSize, true);
            var sequentalResult =
                ExperimentEvaluations.EvaluateMatrixMultiplication(size.firstMatrixSize, size.secondMatrixSize, false);

            cells.Add(new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(
                    $"{size.firstMatrixSize.rowSize}x{size.firstMatrixSize.columnSize} X {size.secondMatrixSize.rowSize}x{size.secondMatrixSize.columnSize}")));
            cells.Add(new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph($"{sequentalResult.average} ± {sequentalResult.standardDeviation} sec")));
            cells.Add(new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph($"{parallelResult.average} ± {parallelResult.standardDeviation} sec")));
        }

        foreach (var cell in cells) table.AddCell(cell);

        document.Add(table);

        document.Close();

        var newPdf = IronPdf.PdfDocument.FromFile("test.pdf");

        newPdf.RasterizeToImageFiles("result.png");
        
        File.Delete("test.pdf");
    }
}