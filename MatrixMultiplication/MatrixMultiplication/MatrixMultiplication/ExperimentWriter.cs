using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace MatrixMultiplication.Experiment;

public static class ExperimentWriter
{
    public static void CreateTable( ((int rowSize, int columnSize) firstMatrixSize, (int rowSize, int columnSize) secondMatrixSize)[] matricesSizes)
    {
        ArgumentNullException.ThrowIfNull(matricesSizes);

        var writer = new PdfWriter("./test.pdf");
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

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
            var parallelResult = ExperimentEvaluations.EvaluateMatrixMultiplication(size.firstMatrixSize, size.secondMatrixSize, true);
            var sequentalResult = ExperimentEvaluations.EvaluateMatrixMultiplication(size.firstMatrixSize, size.secondMatrixSize, false);

            cells.Add(new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph($"{size.firstMatrixSize.rowSize}x{size.firstMatrixSize.columnSize} X {size.secondMatrixSize.rowSize}x{size.secondMatrixSize.columnSize}")));
            cells.Add(new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph($"{sequentalResult.average} ± {sequentalResult.standartDeviation}")));
            cells.Add(new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph($"{parallelResult.average} ± {parallelResult.standartDeviation}")));
        }

        foreach (var cell in cells)
        {
            table.AddCell(cell);
        }

        document.Add(table);

        document.Close();
    }
}

