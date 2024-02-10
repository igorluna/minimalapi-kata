using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using minimal_kata;
using minimal_kata.Data;
using minimal_kata.Models;
using minimal_kata.Models.DTO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//TIP Inject new services here

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/coupon", (IMapper _mapper, ILogger<Program> _logger) => {
    _logger.Log(LogLevel.Information, "Getting all coupons");

    var response = new APIResponse
    {
        Result = _mapper.Map<List<CouponDTO>>(CouponStore.CouponList),
        IsSuccess = true,
        StatusCode = System.Net.HttpStatusCode.OK
    };

    return Results.Ok(response);
})
.WithName("GetCoupons")
.Produces<APIResponse>(StatusCodes.Status200OK);

app.MapGet("/api/coupon/{id:int}", (IMapper _mapper, int id) => {
    var coupon = CouponStore.CouponList.FirstOrDefault(x=>x.Id == id);
    
    APIResponse response;
    if(coupon == null)
    {
        response = new APIResponse
        {
            IsSuccess = false,
            StatusCode = System.Net.HttpStatusCode.NotFound
        };
        return Results.NotFound(response);
    }

    response = new APIResponse
    {
        Result = _mapper.Map<CouponDTO>(coupon),
        IsSuccess = true,
        StatusCode = System.Net.HttpStatusCode.OK
    };

    return Results.Ok(response);
})
.WithName("GetCoupon")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status404NotFound);

app.MapPost("/api/coupon",  async (IMapper _mapper, IValidator<CouponCreateDTO> _validator, [FromBody] CouponCreateDTO couponCreateDto) => {
    var validationResult = await _validator.ValidateAsync(couponCreateDto);
    
    APIResponse response;
    if(!validationResult.IsValid)
    {
        response= new APIResponse{
            
            IsSuccess = false,
            StatusCode = System.Net.HttpStatusCode.BadRequest
        };
        response.ErrorMessages.Add(validationResult.Errors.First().ToString());
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(couponCreateDto);

    coupon.Id = CouponStore.CouponList.OrderByDescending(x => x.Id).First().Id + 1;

    coupon.Created = DateTime.Now;

    CouponStore.CouponList.Add(coupon); 

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    response = new APIResponse
    {
        Result = couponDTO,
        IsSuccess = true,
        StatusCode = System.Net.HttpStatusCode.OK
    };

    return Results.CreatedAtRoute($"GetCoupon",new {id = coupon.Id}, response);
})
.WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
.Produces<APIResponse>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.MapPut("/api/coupon/{id:int}", (IMapper _mapper, int id, [FromBody] CouponUpdateDTO couponUpdateDto) => {
    APIResponse response;
    var coupon = CouponStore.CouponList.FirstOrDefault(x => x.Id == id);
    if(coupon == null)
    {
        response = new APIResponse
        {
            IsSuccess = true,
            StatusCode = System.Net.HttpStatusCode.OK
        };
        response.ErrorMessages.Add("Not found");
        return Results.NotFound(response);
    }

    CouponStore.CouponList.RemoveAll(x => x.Id == id);

    coupon.Name = couponUpdateDto.Name;
    coupon.Percent = couponUpdateDto.Percent;
    coupon.IsActive = couponUpdateDto.IsActive;
    coupon.LastUpdated = DateTime.Now;

    CouponStore.CouponList.Add(coupon);
    
    response = new APIResponse
    {
        Result = _mapper.Map<CouponDTO>(coupon),
        IsSuccess = true,
        StatusCode = System.Net.HttpStatusCode.OK
    };
    return Results.Ok(response);
})
.WithName("UpdateCoupon")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status404NotFound);

app.MapDelete("/api/coupon/{id:int}", (IMapper _mapper, ILogger<Program> _logger, int id)=> {
APIResponse response;

    var coupon = CouponStore.CouponList.FirstOrDefault(x => x.Id == id);

    if(coupon == null)
    {
        response = new APIResponse
        {
            IsSuccess = true,
            StatusCode = System.Net.HttpStatusCode.OK
        };
        response.ErrorMessages.Add("Not found");
        return Results.NotFound(response);
    }

    CouponStore.CouponList.RemoveAll(x => x.Id == id);
    var cuponResponse = _mapper.Map<CouponDTO>(coupon);
    response = new APIResponse
    {
        Result = cuponResponse,
        IsSuccess = true,
        StatusCode = System.Net.HttpStatusCode.OK
    };
    return Results.Ok(response);
    
})
.WithName("DeleteCoupon")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status404NotFound);;

app.Run();
