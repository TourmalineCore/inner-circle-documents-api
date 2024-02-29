using Application;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api")]
public class DocumentsController : Controller
{
    private readonly IInnerCircleHttpClient _client;


    public DocumentsController(IInnerCircleHttpClient client)
    {
        _client = client;
    }
}