using TransactionUpload.Models;
using System.Globalization;
using System.Xml.Linq;

namespace TransactionUpload.Services
{
    public class XmlTransactionParser
    {
        private readonly List<string> _validationMessages;
        private readonly string _logFolderPath;
        public bool IsValid => !_validationMessages.Any();

      
        public XmlTransactionParser(string logFolderPath)
        {
            _validationMessages = new List<string>();
            _logFolderPath = logFolderPath;
        }

        public IEnumerable<TransactionUPD> Parse(string content)
        {
            var transactions = new List<TransactionUPD>();

            try
            {
                var xmlDoc = XDocument.Parse(content);
                var transactionNodes = xmlDoc.Descendants("Transaction");

                foreach (var transactionNode in transactionNodes)
                {
                    var transaction = new TransactionUPD();

                    // Transaction ID
                    transaction.TransactionId = transactionNode.Attribute("id")?.Value;

                    // Transaction Date
                    var transactionDateStr = transactionNode.Element("TransactionDate")?.Value;
                    if (DateTime.TryParse(transactionDateStr, out var transactionDate))
                    {
                        transaction.TransactionDate = transactionDate;
                    }
                    else
                    {
                        _validationMessages.Add($"Invalid data in transaction with id '{transaction.TransactionId}': Invalid Date format.");
                    }

                    // Payment Details
                    var paymentDetailsNode = transactionNode.Element("PaymentDetails");
                    if (paymentDetailsNode != null)
                    {
                        // Amount
                        var amountStr = paymentDetailsNode.Element("Amount")?.Value;
                        if (decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                        {
                            transaction.Amount = amount;
                        }
                        else
                        {
                            _validationMessages.Add($"Invalid data in transaction with id '{transaction.TransactionId}': Missing or Invalid Amount.");
                        }

                        // Currency Code
                        transaction.CurrencyCode = paymentDetailsNode.Element("CurrencyCode")?.Value;
                    }

                    // Status
                    transaction.Status = transactionNode.Element("Status")?.Value;

                    transactions.Add(transaction);
                }
            }
            catch (Exception ex)
            {
                _validationMessages.Add($"Error parsing XML: {ex.Message}");
                LogErrorToFile(ex);
            }

            return transactions;
        }

        private void LogErrorToFile(Exception ex)
        {
            string logFilePath = Path.Combine(_logFolderPath, "ErrorLog.txt");

            using (var writer = new StreamWriter(logFilePath, append: true))
            {
                writer.WriteLine($"Error Time: {DateTime.Now}");
                writer.WriteLine($"Exception Message: {ex.Message}");
                writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                writer.WriteLine(new string('-', 50));
            }
        }

        public IEnumerable<string> GetValidationMessages()
        {
            return _validationMessages;
        }
    }
}
