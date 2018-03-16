using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")] //for routing where [controller] = Todo;  ASP.NET Core routing isn't case sensitive.
    public class TodoController : Controller
    {
        private readonly TodoContext _context;

        //The constructor uses Dependency Injection to inject the database context (TodoContext) into the controller. 
        public TodoController(TodoContext context)
        {
            _context = context;
            //add values
            if(_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }

        // GET: api/<controller>
        //GET /api/todo
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        // GET api/<controller>/5
        // GET /api/todo/{id}
        //Name = "GetTodo" creates a named route.
        //will handle InvalidOperationException: No route matches the supplied values if Name param is not supplied a
        [HttpGet("{id}",Name ="GetTodo")]
        public IActionResult GetById(long id)
        {
            var item = _context.TodoItems.FirstOrDefault(x => x.Id == id);
            if(item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }



        [HttpPost]
        //The [FromBody] attribute tells MVC to get the value of the to-do item from the body of the HTTP request.
        public IActionResult Create([FromBody] TodoItem item) 
        {
            if(item == null)
            {
                return BadRequest();
            }
            _context.TodoItems.Add(item);
            _context.SaveChanges();

            //Returns a 201 response. HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //Uses the "GetTodo" named route to create the URL.
            //The "GetTodo" named route is defined in GetById:
            return CreatedAtRoute("GetTodo", new { id = item.Id }, item); 
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {
            if(item == null || item.Id != id)
            {
                return BadRequest();
            }

            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if(todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = item.IsComplete;
            todo.Name = item.Name;
            _context.TodoItems.Update(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }

        //// POST api/<controller>
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
