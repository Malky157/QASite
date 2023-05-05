using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework5._1.Data
{
    public class QARepository
    {
        private readonly string _connectionString;
        public QARepository(string connection)
        {
            _connectionString = connection;
        }
        public List<Question> GetAllQuestions()
        {
            using var context = new QADbContext(_connectionString);
            return context.Questions
                .Include(u => u.User)
                .Include(a => a.Answers)
                .Include(qt => qt.QuestionsTags)
                .ThenInclude(t => t.Tag)
                .OrderByDescending(q => q.DatePosted)
                .ToList();
        }

        public void AddQuestion(Question question, List<string> tags)
        {
            using var context = new QADbContext(_connectionString);
            context.Questions.Add(question);
            context.SaveChanges();

            foreach (var tag in tags)
            {
                var t = GetTag(tag);
                int tagId;
                if (t == null)
                {
                    tagId = AddTag(tag);
                }
                else
                {
                    tagId = t.Id;
                }
                context.QuestionsTags.Add(new QuestionsTags
                {
                    QuestionId = question.Id,
                    TagId = tagId
                });
            }
            context.SaveChanges();
        }
        public Question GetQuestionForId(int id)
        {
            using var context = new QADbContext(_connectionString);
            return context.Questions
                .Include(u => u.User)
                .Include(a => a.Answers)
                .Include(qt => qt.QuestionsTags)
                .ThenInclude(t => t.Tag)
                .FirstOrDefault(q => q.Id == id);
        }
        private Tag GetTag(string name)
        {
            using var context = new QADbContext(_connectionString);
            return context.Tags.FirstOrDefault(t => t.Name == name);
        }
        private int AddTag(string name)
        {
            using var context = new QADbContext(_connectionString);
            var tag = new Tag { Name = name };
            context.Tags.Add(tag);
            context.SaveChanges();
            return tag.Id;
        }
        public void AddAnswer(Answer answer)
        {
            using var context = new QADbContext(_connectionString);
            context.Answers.Add(answer);
            context.SaveChanges();
        }
    }
}
