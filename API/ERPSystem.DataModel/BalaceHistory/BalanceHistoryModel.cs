using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ERPSystem.DataModel.BalanceHistory
{
    public class BalanceHistoryModel
    {
        public int Id { get; set; }
        public string WalletAddress { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public int UserId { get; set; }

        public string CreatedOn { get; set; }

    }

    public class BalanceHistoryEditModel
    {
        public int Id { get; set; }
        public string WalletAddress { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public int UserId { get; set; }

    }

    public class BalanceHistoryAddModel
    {
        public int Id { get; set; }
        public string WalletAddress { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public int UserId { get; set; }

        public string CreatedOn { get; set; }
    }

    public class BalanceHistoryListModel
    {
        public int Id { get; set; }
        public string WalletAddress { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public int UserId { get; set; }

        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
    }
    public class DataChart
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    

}
