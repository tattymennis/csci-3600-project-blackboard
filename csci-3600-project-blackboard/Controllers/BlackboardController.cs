using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using csci_3600_project_the_struggle.Data;
using csci_3600_project_the_struggle.Models;
using System.IO;
using csci_3600_project_the_struggle.Cryptography; // import crypto ops

namespace csci_3600_project_the_struggle.Controllers
{
    public class BlackboardController : Controller
    {
        public ActionResult BlackboardView()
        {
            BlackboardViewModel vm = new BlackboardViewModel();

            // populate Blackboard
            Blackboard blackboard = vm.Blackboard ?? new Blackboard();
            List<BoardPost> boardposts = blackboard.BoardPosts ?? new List<BoardPost>();

            using (DataModel dm = new DataModel())
            {
                var bp = dm.BoardPosts;

                foreach (BoardPost b in bp)
                {
                    boardposts.Add(b);
                }

                if (blackboard.BoardPosts == null)
                    blackboard.BoardPosts = boardposts;
                else
                    blackboard.BoardPosts.AddRange(boardposts);

                vm.Blackboard = blackboard;
            }

            vm.Encryption = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Select encryption algorithm", Selected = false },
                new SelectListItem { Value = "aes128", Text = "AES-128", Selected = false },
                new SelectListItem { Value = "aes192", Text = "AES-192", Selected = false },
                new SelectListItem { Value = "aes256", Text = "AES-256", Selected = false }
            };
            ViewData["EncryptionAlgos"] = vm.Encryption;

            return View(vm);
        }

        [HttpPost]
        public ActionResult BlackboardView(BlackboardViewModel vm)
        {
            vm.PostTime = DateTime.Now;
            vm.Encryption = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Select encryption algorithm", Selected = false },
                new SelectListItem { Value = "aes128", Text = "AES-128", Selected = false },
                new SelectListItem { Value = "aes192", Text = "AES-192", Selected = false },
                new SelectListItem { Value = "aes256", Text = "AES-256", Selected = false }
            };
            ViewData["EncryptionAlgos"] = vm.Encryption;

            // populate Blackboard (copied from GET)
            Blackboard blackboard = vm.Blackboard ?? new Blackboard();
            List<BoardPost> boardposts = blackboard.BoardPosts ?? new List<BoardPost>();

            using (DataModel dm = new DataModel())
            {
                var bp = dm.BoardPosts;

                foreach (BoardPost b in bp)
                {
                    boardposts.Add(b);
                }

                if (blackboard.BoardPosts == null)
                    blackboard.BoardPosts = boardposts;
                else
                    blackboard.BoardPosts.AddRange(boardposts);

                vm.Blackboard = blackboard;
            }
            
            // Server-side validation
            if (vm.UserHandle.Length < 1 || vm.UserHandle.Length > 256)
                ModelState.AddModelError(nameof(vm.UserHandle), "User handle is invalid.");

            if (vm.EncryptionChoice == null || vm.EncryptionChoice == "")
                ModelState.AddModelError(nameof(vm.EncryptionChoice), "Please select an encryption algorithm.");

            if (vm.Message.Length < 1 || vm.Message.Length > 256)
                ModelState.AddModelError(nameof(vm.Message), "Message content is too long.");

            if (vm.EncryptionKey.Length < 8 || vm.EncryptionKey.Length > 256)
                ModelState.AddModelError(nameof(vm.EncryptionKey), "Password must be between 8 - 256 characters.");

            if (ModelState.IsValid)
            {
                Crypto Crypto = new Crypto();
                string plaintxt = vm.Message;
                string key = vm.EncryptionKey;
                string ciphertxt;

                switch (vm.EncryptionChoice)
                {
                    case ("aes128"):
                        ciphertxt = (string)Crypto.Crypt(CryptType.Encrypt, CryptAlgo.AES128, plaintxt, key);
                        ViewData["encryptedMsg"] = ciphertxt;
                        break;

                    case ("aes192"):
                        ciphertxt = (string)Crypto.Crypt(CryptType.Encrypt, CryptAlgo.AES192, plaintxt, key);
                        ViewData["encryptedMsg"] = ciphertxt;
                        break;

                    case ("aes256"):
                        ciphertxt = (string)Crypto.Crypt(CryptType.Encrypt, CryptAlgo.AES256, plaintxt, key);
                        ViewData["encryptedMsg"] = ciphertxt;
                        break;

                    default:
                        throw new Exception("Something went very wrong.");
                }

                // collect data from form
                BoardPost bp = new BoardPost();
                bp.Content = ciphertxt; // write encrypted content to BoardPost, not original Content
                bp.PostTime = vm.PostTime;
                bp.Poster = vm.UserHandle;
                bp.TimeToLive = 600000; // 600000 ms = 10 minutes 
                bp.EncryptionAlgorithm = vm.EncryptionChoice;
                bp.Salt = Crypto.Salt;
                bp.IV = Crypto.IV;

                vm.Blackboard.BoardPosts.Add(bp); // add post to view model

                // package BoardPost object to be written to database
                using (DataModel dm = new DataModel())
                {
                    dm.BoardPosts.Add(bp);
                    dm.SaveChanges();
                }

                return RedirectToAction("BlackboardView", "Blackboard");
            }
            return View(vm);
        }

        public ActionResult GetAjaxPlaintext(JsonDecryptionModel jdm)
        {
            JsonResult json = new JsonResult();

            int id;
            bool succeeded = int.TryParse(jdm.Id, out id);
            string password = jdm.Password; // replace

            string ciphertxt; // replace       
            string plaintext = ""; // output
            string algo;
            byte[] salt;
            byte[] iv;

            try
            {
                using (DataModel dm = new Data.DataModel())
                {
                    BoardPost bp = dm.BoardPosts.Where(x => x.Id == id).FirstOrDefault();
                    ciphertxt = bp.Content;
                    algo = bp.EncryptionAlgorithm;
                    salt = bp.Salt;
                    iv = bp.IV;
                }

                Crypto Crypto = new Crypto();
                Crypto.Salt = salt;
                Crypto.IV = iv;

                switch (algo.ToLower())
                {
                    case ("aes128"):
                        plaintext = (string)Crypto.Crypt(CryptType.Decrypt, CryptAlgo.AES128, ciphertxt, password);
                        break;

                    case ("aes192"):
                        plaintext = (string)Crypto.Crypt(CryptType.Decrypt, CryptAlgo.AES192, ciphertxt, password);
                        break;

                    case ("aes256"):
                        plaintext = (string)Crypto.Crypt(CryptType.Decrypt, CryptAlgo.AES256, ciphertxt, password);
                        break;

                    default:
                        throw new Exception("Something went very wrong.");
                }
            }

            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return Json(new { success = true, ptxt = plaintext }, JsonRequestBehavior.AllowGet);
        }
    }
}