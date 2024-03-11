using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AuctionService.Entities;

[Table("Auctions")]
public class Auction
{

public Guid Id { get; set;}

public int ReservePrice { get; set;} = 0;
public string Seller { get; set;}
public string Winner { get; set;}
public int? SoldAmount { get; set;}
public int? CurrenthighBig { get; set;}
public DateTime CreatedAt { get; set;}= DateTime.UtcNow;
public DateTime UpdatedAt { get; set;}= DateTime.UtcNow;
public DateTime AuctionEnd { get; set;}= DateTime.UtcNow;
public Status Status { get; set;}
public Item Item { get; set;}



}