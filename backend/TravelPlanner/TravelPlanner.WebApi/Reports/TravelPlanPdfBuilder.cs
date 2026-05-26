using System.Globalization;
using System.Text;
using TravelPlanner.Common.Models;

namespace TravelPlanner.WebApi.Reports
{
    public static class TravelPlanPdfBuilder
    {
        private const float PageWidth = 595f;
        private const float PageHeight = 842f;
        private const float Margin = 42f;

        public static byte[] Build(
            TravelPlanData plan,
            IReadOnlyCollection<DestinationData> destinations,
            IReadOnlyCollection<ActivityData> activities,
            IReadOnlyCollection<ExpenseData> expenses,
            IReadOnlyCollection<ChecklistItemData> checklistItems,
            IReadOnlyCollection<ReminderData> reminders)
        {
            var renderer = new PdfReportRenderer();
            var culture = CultureInfo.InvariantCulture;

            var expensesTotal = expenses.Sum(item => item.Amount);
            var activitiesTotal = activities.Sum(item => item.EstimatedCost);
            var committedTotal = expensesTotal + activitiesTotal;
            var remainingBudget = plan.PlannedBudget - committedTotal;

            renderer.AddHeader(plan);
            renderer.AddSummaryCards(
                ("Period", $"{plan.StartDate:yyyy-MM-dd} do {plan.EndDate:yyyy-MM-dd}"),
                ("Budzet", $"{plan.PlannedBudget.ToString("0.00", culture)} EUR"),
                ("Preostalo", $"{remainingBudget.ToString("0.00", culture)} EUR"));

            renderer.AddParagraphSection("Opis putovanja", string.IsNullOrWhiteSpace(plan.Description)
                ? "Nije dodat opis putovanja."
                : plan.Description);

            renderer.AddParagraphSection("Napomene", string.IsNullOrWhiteSpace(plan.Notes)
                ? "Nema dodatnih napomena."
                : plan.Notes);

            renderer.AddListSection(
                $"Destinacije ({destinations.Count})",
                destinations
                    .OrderBy(item => item.ArrivalDate)
                    .SelectMany(item => BuildDestinationBlock(item))
                    .ToList(),
                "Nema unesenih destinacija.");

            renderer.AddListSection(
                $"Aktivnosti ({activities.Count})",
                activities
                    .OrderBy(item => item.Date)
                    .ThenBy(item => item.Time)
                    .SelectMany(item => BuildActivityBlock(item, culture))
                    .ToList(),
                "Nema planiranih aktivnosti.");

            renderer.AddListSection(
                $"Troskovi ({expenses.Count})",
                BuildExpenseLines(expenses, expensesTotal, culture),
                "Nema evidentiranih troskova.");

            renderer.AddListSection(
                $"Podsjetnici ({reminders.Count})",
                reminders
                    .OrderBy(item => item.RemindAt)
                    .Select(item =>
                        $"{item.RemindAt:yyyy-MM-dd HH:mm}  |  {item.Title}  |  {(item.IsCompleted ? "Zavrseno" : "Aktivno")}")
                    .ToList(),
                "Nema aktivnih podsjetnika.");

            renderer.AddListSection(
                $"Lista stvari ({checklistItems.Count})",
                checklistItems
                    .OrderBy(item => item.IsCompleted)
                    .ThenBy(item => item.Text)
                    .Select(item => $"{item.Text}  |  {(item.IsCompleted ? "Zavrseno" : "U toku")}")
                    .ToList(),
                "Nema dodatih stavki.");

            return BuildPdf(renderer.BuildPages());
        }

        private static List<string> BuildDestinationBlock(DestinationData destination)
        {
            var lines = new List<string>
            {
                $"{destination.Name}  |  {destination.Location}",
                $"Boravak: {destination.ArrivalDate:yyyy-MM-dd} - {destination.DepartureDate:yyyy-MM-dd}"
            };

            if (!string.IsNullOrWhiteSpace(destination.Description))
            {
                lines.Add(destination.Description);
            }

            lines.Add(string.Empty);
            return lines;
        }

        private static List<string> BuildActivityBlock(ActivityData activity, CultureInfo culture)
        {
            var lines = new List<string>
            {
                $"{activity.Date:yyyy-MM-dd}  |  {activity.Time:hh\\:mm}  |  {activity.Name}",
                $"Lokacija: {activity.Location}",
                $"Status: {activity.Status}  |  Procijenjeni trosak: {activity.EstimatedCost.ToString("0.00", culture)} EUR"
            };

            if (!string.IsNullOrWhiteSpace(activity.Description))
            {
                lines.Add(activity.Description);
            }

            lines.Add(string.Empty);
            return lines;
        }

        private static List<string> BuildExpenseLines(
            IReadOnlyCollection<ExpenseData> expenses,
            decimal expensesTotal,
            CultureInfo culture)
        {
            var lines = new List<string>
            {
                $"Ukupan iznos troskova: {expensesTotal.ToString("0.00", culture)} EUR",
                string.Empty
            };

            foreach (var expense in expenses.OrderBy(item => item.Date))
            {
                lines.Add($"{expense.Date:yyyy-MM-dd}  |  {expense.Name}  |  {expense.Category}  |  {expense.Amount.ToString("0.00", culture)} EUR");

                if (!string.IsNullOrWhiteSpace(expense.Description))
                {
                    lines.Add(expense.Description);
                }

                lines.Add(string.Empty);
            }

            return lines;
        }

        private static byte[] BuildPdf(List<string> pageStreams)
        {
            var objects = new List<string>();
            var pageObjectNumbers = new List<int>();
            const int fontRegularObjectNumber = 1;
            const int fontBoldObjectNumber = 2;
            const int pagesObjectNumber = 3;
            var nextObjectNumber = 4;

            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");
            objects.Add(string.Empty);

            foreach (var stream in pageStreams)
            {
                var contentObjectNumber = nextObjectNumber++;
                var pageObjectNumber = nextObjectNumber++;
                pageObjectNumbers.Add(pageObjectNumber);

                objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}\nendstream");
                objects.Add(
                    $"<< /Type /Page /Parent {pagesObjectNumber} 0 R /MediaBox [0 0 {PageWidth.ToString(CultureInfo.InvariantCulture)} {PageHeight.ToString(CultureInfo.InvariantCulture)}] /Resources << /Font << /F1 {fontRegularObjectNumber} 0 R /F2 {fontBoldObjectNumber} 0 R >> >> /Contents {contentObjectNumber} 0 R >>");
            }

            objects[2] = $"<< /Type /Pages /Kids [{string.Join(" ", pageObjectNumbers.Select(number => $"{number} 0 R"))}] /Count {pageObjectNumbers.Count} >>";
            objects.Add("<< /Type /Catalog /Pages 3 0 R >>");

            return SerializePdf(objects);
        }

        private static byte[] SerializePdf(List<string> objects)
        {
            var builder = new StringBuilder();
            builder.Append("%PDF-1.4\n");

            var offsets = new List<int> { 0 };

            for (var index = 0; index < objects.Count; index++)
            {
                offsets.Add(Encoding.ASCII.GetByteCount(builder.ToString()));
                builder.Append($"{index + 1} 0 obj\n");
                builder.Append(objects[index]);
                builder.Append("\nendobj\n");
            }

            var xrefPosition = Encoding.ASCII.GetByteCount(builder.ToString());
            builder.Append($"xref\n0 {objects.Count + 1}\n");
            builder.Append("0000000000 65535 f \n");

            foreach (var offset in offsets.Skip(1))
            {
                builder.Append($"{offset:0000000000} 00000 n \n");
            }

            builder.Append("trailer\n");
            builder.Append($"<< /Size {objects.Count + 1} /Root {objects.Count} 0 R >>\n");
            builder.Append("startxref\n");
            builder.Append($"{xrefPosition}\n");
            builder.Append("%%EOF");

            return Encoding.ASCII.GetBytes(builder.ToString());
        }

        private static string EscapePdfText(string value)
        {
            return Transliterate(value)
                .Replace("\\", "\\\\", StringComparison.Ordinal)
                .Replace("(", "\\(", StringComparison.Ordinal)
                .Replace(")", "\\)", StringComparison.Ordinal);
        }

        private static string Transliterate(string value)
        {
            return value
                .Replace('č', 'c')
                .Replace('ć', 'c')
                .Replace('ž', 'z')
                .Replace('š', 's')
                .Replace('đ', 'd')
                .Replace('Č', 'C')
                .Replace('Ć', 'C')
                .Replace('Ž', 'Z')
                .Replace('Š', 'S')
                .Replace('Đ', 'D');
        }

        private sealed class PdfReportRenderer
        {
            private readonly List<StringBuilder> _pages = [];
            private StringBuilder _currentPage = new();
            private float _cursorY = PageHeight - Margin;

            public PdfReportRenderer()
            {
                _pages.Add(_currentPage);
            }

            public void AddHeader(TravelPlanData plan)
            {
                DrawFilledRectangle(Margin, PageHeight - 122, PageWidth - (Margin * 2), 80, 0.07f, 0.10f, 0.19f);
                DrawFilledRectangle(PageWidth - 180, PageHeight - 112, 96, 60, 0.98f, 0.79f, 0.15f);

                DrawText("Travel Planner", Margin + 18, PageHeight - 74, 24, bold: true, r: 1f, g: 1f, b: 1f);
                DrawText("Izvjestaj plana putovanja", Margin + 18, PageHeight - 96, 11, bold: false, r: 0.83f, g: 0.88f, b: 0.96f);
                DrawText(plan.Title, Margin + 18, PageHeight - 112, 18, bold: true, r: 0.99f, g: 0.82f, b: 0.18f);

                DrawText("BUDZET", PageWidth - 166, PageHeight - 74, 10, bold: true, r: 0.11f, g: 0.14f, b: 0.22f);
                DrawText($"{plan.PlannedBudget.ToString("0.00", CultureInfo.InvariantCulture)} EUR", PageWidth - 166, PageHeight - 96, 16, bold: true, r: 0.07f, g: 0.10f, b: 0.19f);

                _cursorY = PageHeight - 148;
            }

            public void AddSummaryCards(params (string Label, string Value)[] cards)
            {
                EnsureSpace(86);

                var cardWidth = (PageWidth - (Margin * 2) - 24) / 3f;
                var x = Margin;

                foreach (var card in cards)
                {
                    DrawFilledRectangle(x, _cursorY - 72, cardWidth, 60, 0.95f, 0.97f, 1f);
                    DrawStrokedRectangle(x, _cursorY - 72, cardWidth, 60, 0.86f, 0.90f, 0.96f);
                    DrawText(card.Label.ToUpperInvariant(), x + 14, _cursorY - 28, 9, bold: true, r: 0.34f, g: 0.43f, b: 0.58f);
                    DrawText(card.Value, x + 14, _cursorY - 52, 14, bold: true, r: 0.07f, g: 0.10f, b: 0.19f);
                    x += cardWidth + 12;
                }

                _cursorY -= 92;
            }

            public void AddParagraphSection(string title, string text)
            {
                var lines = WrapText(text, 84);
                var height = 28 + Math.Max(22, lines.Count * 14) + 18;
                EnsureSpace(height);

                DrawSectionTitle(title);
                DrawFilledRectangle(Margin, _cursorY - (18 + (lines.Count * 14)), PageWidth - (Margin * 2), lines.Count * 14 + 14, 0.99f, 0.99f, 0.99f);
                DrawStrokedRectangle(Margin, _cursorY - (18 + (lines.Count * 14)), PageWidth - (Margin * 2), lines.Count * 14 + 14, 0.90f, 0.92f, 0.95f);

                var textY = _cursorY - 18;
                foreach (var line in lines)
                {
                    DrawText(line, Margin + 14, textY, 11, bold: false, r: 0.20f, g: 0.24f, b: 0.31f);
                    textY -= 14;
                }

                _cursorY = textY - 18;
            }

            public void AddListSection(string title, List<string> lines, string emptyState)
            {
                var preparedLines = lines.Count == 0 ? new List<string> { emptyState } : WrapLines(lines, 86);

                DrawSectionTitle(title);

                foreach (var line in preparedLines)
                {
                    var lineHeight = string.IsNullOrEmpty(line) ? 9 : 15;
                    EnsureSpace(lineHeight + 10);

                    if (string.IsNullOrEmpty(line))
                    {
                        _cursorY -= 8;
                        continue;
                    }

                    DrawText(line, Margin + 8, _cursorY - 14, 10.5f, bold: false, r: 0.18f, g: 0.21f, b: 0.28f);
                    _cursorY -= lineHeight;
                }

                _cursorY -= 14;
            }

            public List<string> BuildPages()
            {
                return _pages.Select(page => page.ToString()).ToList();
            }

            private void DrawSectionTitle(string title)
            {
                EnsureSpace(36);
                DrawFilledRectangle(Margin, _cursorY - 22, PageWidth - (Margin * 2), 22, 0.98f, 0.79f, 0.15f);
                DrawText(title, Margin + 10, _cursorY - 15, 11, bold: true, r: 0.07f, g: 0.10f, b: 0.19f);
                _cursorY -= 34;
            }

            private void EnsureSpace(float height)
            {
                if (_cursorY - height >= Margin)
                {
                    return;
                }

                _currentPage = new StringBuilder();
                _pages.Add(_currentPage);
                _cursorY = PageHeight - Margin;
            }

            private void DrawFilledRectangle(float x, float y, float width, float height, float r, float g, float b)
            {
                _currentPage.AppendLine($"{Format(r)} {Format(g)} {Format(b)} rg");
                _currentPage.AppendLine($"{Format(x)} {Format(y)} {Format(width)} {Format(height)} re f");
            }

            private void DrawStrokedRectangle(float x, float y, float width, float height, float r, float g, float b)
            {
                _currentPage.AppendLine($"{Format(r)} {Format(g)} {Format(b)} RG");
                _currentPage.AppendLine($"{Format(x)} {Format(y)} {Format(width)} {Format(height)} re S");
            }

            private void DrawText(string text, float x, float y, float fontSize, bool bold, float r, float g, float b)
            {
                _currentPage.AppendLine("BT");
                _currentPage.AppendLine($"/F{(bold ? 2 : 1)} {Format(fontSize)} Tf");
                _currentPage.AppendLine($"{Format(r)} {Format(g)} {Format(b)} rg");
                _currentPage.AppendLine($"{Format(x)} {Format(y)} Td");
                _currentPage.AppendLine($"({EscapePdfText(text)}) Tj");
                _currentPage.AppendLine("ET");
            }

            private static List<string> WrapText(string text, int maxLength)
            {
                return WrapLines([text], maxLength);
            }

            private static List<string> WrapLines(IEnumerable<string> source, int maxLength)
            {
                var wrapped = new List<string>();

                foreach (var line in source)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        wrapped.Add(string.Empty);
                        continue;
                    }

                    var remaining = line.Trim();
                    while (remaining.Length > maxLength)
                    {
                        var splitIndex = remaining.LastIndexOf(' ', maxLength);
                        if (splitIndex <= 0)
                        {
                            splitIndex = maxLength;
                        }

                        wrapped.Add(remaining[..splitIndex].TrimEnd());
                        remaining = remaining[splitIndex..].TrimStart();
                    }

                    if (remaining.Length > 0)
                    {
                        wrapped.Add(remaining);
                    }
                }

                return wrapped;
            }

            private static string Format(float value)
            {
                return value.ToString("0.##", CultureInfo.InvariantCulture);
            }
        }
    }
}
