using ChatApp.Database;
using ChatApp.Hubs;
using ChatApp.Infrastructure;
using ChatApp.Models;
using ChatApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IHubContext<ChatHub> _chat;
        private readonly AppDbContext _context;
        private readonly IChatRepository _repo;
        public HomeController(AppDbContext context, IChatRepository repo, IHubContext<ChatHub> chat)
        {
            _context = context;
            _repo = repo;
            _chat = chat;
        }
        public IActionResult Index()
        {
            var chats = _repo.GetChats(GetUserId());

            return View(chats);
        }

        public IActionResult Find()
        {
            var users = _context.Users
                    .Where(x => x.Id != User.GetUserId())
                    .ToList();

            return View(users);
        }


        public IActionResult Private()
        {
            var chats = _repo.GetPrivateChats(GetUserId());

            return View(chats);
        }
        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {

            var id = await _repo.CreatePrivateRoom(GetUserId(), userId);

            return RedirectToAction("Chat", new { id });
        }

        [HttpGet("{id}")]
        public IActionResult Chat(int id)
        {
            return View(_repo.GetChat(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {

            await _repo.CreateRoom(name, GetUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> JoinRoom(int id)
        {
            await _repo.JoinRoom(id, GetUserId());
            return RedirectToAction("Chat", "Home", new { id = id  });
        }

        public async Task<IActionResult> SendMessage(int roomId, string message)
        {
            var Message = await _repo.CreateMessage(roomId, message, User.Identity.Name);

            await _chat.Clients.Group(roomId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    Text = Message.Text,
                    Name = Message.Name,
                    Timestamp = Message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss")
                });

            return Ok();
        }
    }
}