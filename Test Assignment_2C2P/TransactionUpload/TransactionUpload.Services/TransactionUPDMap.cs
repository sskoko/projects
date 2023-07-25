using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionUpload.Models;

namespace TransactionUpload.Services
{
    // CsvHelper ClassMap for TransactionUPD model
    public sealed class TransactionUPDMap : ClassMap<TransactionUPD>
    {
        public TransactionUPDMap()
        {
            // Map the CSV fields to the TransactionUPD properties
            Map(m => m.TransactionId).Index(0).Name("TransactionId");
            Map(m => m.Amount).Index(1).Name("Amount");
            Map(m => m.CurrencyCode).Index(2).Name("CurrencyCode");
            Map(m => m.TransactionDate).Index(3).Name("TransactionDate").TypeConverterOption.Format("dd/MM/yyyy HH:mm:ss");
            Map(m => m.Status).Index(4).Name("Status");
        }
    }
}
