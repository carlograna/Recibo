using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReceiptExport
{
    class RecordType01
    {
        public RecordType01() { }

        private enum FieldLength : int
        {
            RecordType = 2,
            RecordCount = 7,
            TotalAmount = 15,
            FirstTimeRecordCount = 7,
            FirstTimeAmount = 15,
            RetransmittalRecordCount = 7,
            RetransmittalAmount = 15,
            CreationStamp = 26,
            Filler = 206

        }

        private string recordType = "01";
        private int recordCount;
        private double totalAmount;
        private int firstTimeRecordCount;
        private double firstTimeAmount;
        private int retransmittalRecordCount;
        private double retransmittalAmount;
        private DateTime creationStamp;
        private string filler = "";

        public int RecordCount
        {
            get { return recordCount; }
            set { recordCount = value; }
        }

        public double TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }

        public int FirstTimeRecordCount
        {
            get { return firstTimeRecordCount; }
            set { firstTimeRecordCount = value; }
        }

        public double FirstTimeAmount
        {
            get { return firstTimeAmount; }
            set { firstTimeAmount = value; }
        }

        public int RetransmittalRecordCount
        {
            get { return retransmittalRecordCount; }
            set { retransmittalRecordCount = value; }
        }

        public double RetransmittalAmount
        {
            get { return retransmittalAmount; }
            set { retransmittalAmount = value; }
        }

        public DateTime CreationStamp
        {
            get {
                return creationStamp; }
            set { creationStamp = value; }
        }

        public String Filler
        {
            get { return filler; }
            set { filler = value; }
        }

        public string RecordLine()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(recordType.PadRight(2));
            sb.Append(recordCount.ToString().PadLeft(7, '0'));
            sb.Append(totalAmount.ToString().PadRight(15));
            sb.Append(firstTimeRecordCount.ToString().PadRight(7));
            sb.Append(firstTimeAmount.ToString().PadRight(15));
            sb.Append(retransmittalRecordCount.ToString().PadRight(7));
            sb.Append(retransmittalAmount.ToString().PadRight(15));
            sb.Append(creationStamp.ToString("yyyy-MM-dd.hh:mm:ss.ffffff").PadRight(26));
            sb.Append(filler.PadRight(173));

            return sb.ToString();
        }
    }
}
