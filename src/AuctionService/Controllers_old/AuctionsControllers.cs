using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;


[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper){
        _context = context;
        _mapper = mapper;
    }

// http://localhost:7001/api/auctions
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        Console.WriteLine(" GetAllAuctions() was called! ");
        var auctions = await _context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        return _mapper.Map<List<AuctionDto>>(auctions);
    }

//http://localhost:7001/api/auctions/?id=1
    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x=>x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }

//http://localhost:7001/api/auctions/?id=1
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {


        var auction = _mapper.Map<Auction>(auctionDto);
        auction.Seller = "TEST";

        _context.Auctions.Add(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not save auction");

        return CreatedAtAction(nameof(GetAuctionById), 
            new {auction.Id}, _mapper.Map<AuctionDto>(auction));
    }

    //http://localhost:7001/api/auctions/?id=1
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto auctionDto)
    {

         var auction = await _context.Auctions.Include(x=>x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound();

        // TODO: check seller username

        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year;

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();
        return BadRequest("Someting is missing in the request");

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {

         var auction = await _context.Auctions.FindAsync(id);

        if(auction == null) return NotFound();

        // TODO: check seller username

        _context.Remove(auction);
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();
        return BadRequest("Could not update DB!");

    }
}