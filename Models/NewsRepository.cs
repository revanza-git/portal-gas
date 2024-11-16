using Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class NewsRepository : INewsRepository
    {
        private ApplicationDbContext context;
        public NewsRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<News> News => context.News;

        public void Save(News news)
        {
            if (news.NewsID == 0)
            {
                news.CreatedOn = DateTime.Now;
                context.News.Add(news);
            }
            else
            {
                News search = context.News.FirstOrDefault(x => x.NewsID == news.NewsID);
                if (search != null)
                {
                    search.Subject = news.Subject;
                    search.Content = news.Content;
                }
            }
            context.SaveChanges();
        }

        public void Submit(News news)
        {
            if (news.NewsID == 0)
            {
                news.CreatedOn = DateTime.Now;
                news.Status = 1;
                context.News.Add(news);
            }
            else
            {
                News search = context.News.FirstOrDefault(x => x.NewsID == news.NewsID);
                if (search != null)
                {
                    search.Status = 1;
                    search.Subject = news.Subject;
                    search.Content = news.Content;
                }
            }
            context.SaveChanges();
        }

        public void Publish(News news)
        {
            if (news.NewsID == 0)
            {
                news.CreatedOn = DateTime.Now;
                news.Status = 2;
                context.News.Add(news);
            }
            else
            {
                News search = context.News.FirstOrDefault(x => x.NewsID == news.NewsID);
                if (search != null)
                {
                    search.Status = 2;
                    search.Subject = news.Subject;
                    search.Content = news.Content;
                }
            }
            context.SaveChanges();
        }

        public void Delete(News news)
        {
            context.News.Remove(news);
            context.SaveChanges();
        }
    }
}
