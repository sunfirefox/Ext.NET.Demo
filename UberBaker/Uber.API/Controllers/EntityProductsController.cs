﻿using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using Uber.Core;
using Uber.Data;

namespace Uber.API.Controllers
{
    public class EntityProductsController : EntitySetController<Product, int>
    {
        private UberContext data = new UberContext();

        [Queryable]
        public override IQueryable<Product> Get()
        {
            return data.Products;
        }

        protected override Product GetEntityByKey(int key)
        {
            Product product = data.Products.Find(key);

            if (product == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return product;
        }

        public override void Delete(int key)
        {
            Product product = data.Products.Find(key);

            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            data.Products.Remove(product);

            try
            {
                data.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
            }
        }

        protected override Product PatchEntity(int key, Delta<Product> patch)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            Product product = data.Products.Find(key);

            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            patch.Patch(product);
            data.Entry(product).State = EntityState.Modified;
            try
            {
                data.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
            }            
            
            
            return product;
        }

        protected override Product UpdateEntity(int key, Product update)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            Product product = data.Products.Find(key);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            data.Entry(product).CurrentValues.SetValues(update);
            try
            {
                data.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
            }        

            return update;
        }

        protected override Product CreateEntity(Product entity)
        {
            if (ModelState.IsValid)
            {
                data.Products.Add(entity);
                data.SaveChanges();
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            return entity;
        }

        protected override int GetKey(Product entity)
        {
            return entity.Id;
        }

        protected override void Dispose(bool disposing)
        {
            data.Dispose();
            base.Dispose(disposing);
        }
    }
}
