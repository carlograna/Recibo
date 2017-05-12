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


        public int ParseAmtToChartsFormat<T>(T amount)
        {
            decimal decAmount;
            int intAmount;

            try
            {
                if (decimal.TryParse(amount.ToString(), out decAmount))
                {
                    intAmount = (int)(decAmount * 100);
                }
                else
                    throw new CustomException("Unable to parse Amount");

                return intAmount;
            }
            catch { throw; }
        }

        public string RecordLine()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(recordType.PadRight(2));
            sb.Append(recordCount.ToString().PadLeft(7, '0'));
            sb.Append(ParseAmtToChartsFormat(totalAmount).ToString().PadLeft(15, '0'));
            sb.Append(firstTimeRecordCount.ToString().PadLeft(7, '0'));
            sb.Append(ParseAmtToChartsFormat(firstTimeAmount).ToString().PadLeft(15, '0'));
            sb.Append(retransmittalRecordCount.ToString().PadLeft(7, '0'));
            sb.Append(ParseAmtToChartsFormat(retransmittalAmount).ToString().PadLeft(15, '0'));
            sb.Append(creationStamp.ToString("yyyy-MM-dd.hh:mm:ss.ffffff").PadRight(26));
            sb.Append(filler.PadRight(206));

            return sb.ToString();
        }
    }
}
