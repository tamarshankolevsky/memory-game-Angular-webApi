﻿using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{
    [RoutePrefix("api/user")]
    [EnableCors("*", "*", "*")]
    public class UserController : ApiController
    {

        [HttpPost]
        [Route("signIn")]
        public HttpResponseMessage SignIn(User newUser)
        {
            try
            {
                lock (DB.Users)
                {

                    if (ModelState.IsValid)
                    {
                        DB.Users.Add(newUser);
                        return Request.CreateResponse(HttpStatusCode.Created, true);
                    }
                }
                List<string> errors = new List<string>();
                ModelState.Values.ToList()
                    .ForEach(value => value.Errors.ToList()
                    .ForEach(err => errors.Add(err.ErrorMessage)));
                return Request.CreateResponse(HttpStatusCode.BadRequest, errors);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpGet]
        [Route("getPartners")]
        public HttpResponseMessage GetPartners()
        {
            try
            {
                List<User> partners = DB.Users.Where(user => user.PartnerUserName == null).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, partners);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        [Route("getUser")]
        public HttpResponseMessage GetUser(string userName)
        {
            try
            {
                User currentUser = DB.Users.First(user => user.UserName.Equals(userName));
                return Request.CreateResponse(HttpStatusCode.OK, currentUser);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpPut]
        [Route("setPartner")]
        public HttpResponseMessage SetPartner()
        {
            try
            {
                lock (DB.Users)
                {
                    string currentUserName = HttpContext.Current.Request.Form["currentUserName"];
                    string partnerUserName = HttpContext.Current.Request.Form["partnerUserName"];
                    if (currentUserName.Equals(partnerUserName))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "you cannot play with yourself");
                    }
                    User currentUser = DB.Users.FirstOrDefault(user => user.UserName.Equals(currentUserName));
                    User partnerUser = DB.Users.FirstOrDefault(user => user.UserName.Equals(partnerUserName));
                    if (currentUser != null &&
                        partnerUser != null &&
                        currentUser.PartnerUserName == null &&
                        partnerUser.PartnerUserName == null)
                    {
                        currentUser.PartnerUserName = partnerUser.UserName;
                        partnerUser.PartnerUserName = currentUser.UserName;
                        Game game = new Game() { Player1 = currentUser, Player2 = partnerUser, CurrentTurn = currentUser.UserName };
                        DB.Games.Add(game);
                        return Request.CreateResponse(HttpStatusCode.Created, game);
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "error");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
