using TransactionUpload.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;

namespace TransactionUpload.Services
{
    public class CsvTransactionParser
    {
        private readonly List<string> _validationMessages;
        private readonly string _logFolderPath;
        public bool IsValid => !_validationMessages.Any();

        public CsvTransactionParser(string logFolderPath)
        {
            _validationMessages = new List<string>();
            _logFolderPath = logFolderPath;
        }

        public IEnumerable<TransactionUPD> Parse(string content)
        {
            var transactions = new List<TransactionUPD>();

            try
            {
                using (var reader = new StringReader(content))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    //csv.Context.Configuration.HasHeaderRecord = false; // CSV doesn't have header row
                    csv.Context.Configuration.TrimOptions = TrimOptions.Trim; // Trim spaces from fields

                    // Map the CSV fields to the TransactionUPD properties
                    csv.Context.RegisterClassMap<TransactionUPDMap>();

                    // Read the records from the CSV
                    transactions = csv.GetRecords<TransactionUPD>().ToList();
                }
            }
            catch (Exception ex)
            {
                _validationMessages.Add($"Error parsing CSV: {ex.Message}");
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
