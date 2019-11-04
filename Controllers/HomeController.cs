using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using MVCHomework.Models;

namespace MVCHomework.Controllers
{
    public class HomeController : Controller
    {
        private const string _path = "App_Data//Notes.json";

        public HomeController()
        {
            if(!System.IO.File.Exists(_path))
                System.IO.File.Create(_path);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAllNotes()
        {
            List<NoteVM> listOfNotes = JsonConvert.DeserializeObject<List<NoteVM>>(ReadJsonFile());
            listOfNotes = (from n in listOfNotes
                          orderby n.Id
                          select n).ToList();
            return View(listOfNotes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(NoteVM model)
        {
            List<NoteVM> listOfNotes = JsonConvert.DeserializeObject<List<NoteVM>>(ReadJsonFile());
            if (listOfNotes == null || listOfNotes.Count == 0)
                model.Id = 1;
            else
                model.Id = listOfNotes.Max(n => n.Id) + 1;
            model.DateTime = DateTime.Now;
            listOfNotes.Add(model);
            WriteJsonFile(JsonConvert.SerializeObject(listOfNotes));
            return RedirectToAction("GetAllNotes");
        }

        public IActionResult Edit(int id)
        {
            List<NoteVM> listOfNotes = JsonConvert.DeserializeObject<List<NoteVM>>(ReadJsonFile());
            NoteVM note = (from n in listOfNotes
                          where n.Id == id
                          select n).First();
            return View(note);
        }

        [HttpPost]
        public IActionResult Edit(NoteVM model)
        {
            List<NoteVM> listOfNotes = JsonConvert.DeserializeObject<List<NoteVM>>(ReadJsonFile());
            NoteVM note = (from n in listOfNotes
                           where n.Id == model.Id
                           select n).First();
            listOfNotes.Remove(note);
            model.DateTime = DateTime.Now;
            listOfNotes.Add(model);
            WriteJsonFile(JsonConvert.SerializeObject(listOfNotes));
            return RedirectToAction("GetAllNotes");
        }

        public IActionResult Delete(int id)
        {
            List<NoteVM> listOfNotes = JsonConvert.DeserializeObject<List<NoteVM>>(ReadJsonFile());
            NoteVM note = (from n in listOfNotes
                           where n.Id == id
                           select n).First();
            return View(note);
        }

        [HttpPost]
        public IActionResult Delete(NoteVM model)
        {
            List<NoteVM> listOfNotes = JsonConvert.DeserializeObject<List<NoteVM>>(ReadJsonFile());
            NoteVM note = (from n in listOfNotes
                           where n.Id == model.Id
                           select n).First();
            listOfNotes.Remove(note);
            WriteJsonFile(JsonConvert.SerializeObject(listOfNotes));
            return RedirectToAction("GetAllNotes");
        }

        private string ReadJsonFile(string path = _path)
        {
            string jsonText;
            using (StreamReader sr = new StreamReader(path))
            {
                jsonText = sr.ReadToEnd();
            }
            return jsonText;
        }

        private void WriteJsonFile(string text, string path = _path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(text);
            }
        }
    }
}
