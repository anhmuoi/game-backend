using System.Collections.Generic;

namespace ERPSystem.DataAccess.Models;

public class BalanceHistory : Base
{
    public int Id { get; set; }
    public string WalletAddress { get; set; }
    public string Name { get; set; }
    public double Balance { get; set; }
    public int UserId { get; set; }
    
}