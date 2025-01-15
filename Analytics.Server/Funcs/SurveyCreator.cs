using Analytics.Server.Objects;
using Npgsql;

namespace Analytics.Server.Funcs
{
    public class SurveyCreator
    {
        public static void InsertSurvey(Survey survey)
        {
            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    string sql = @"
                    INSERT INTO Surveys (title, description, image_url, creation_date, end_date) 
                    VALUES 
                    (@title, @description, @image_url, @creation_date, @end_date) 
                    RETURNING survey_id;";

                    using (var commandSurvey = new NpgsqlCommand(sql, connection))
                    {
                        commandSurvey.Parameters.AddWithValue("@title", survey.Title);
                        commandSurvey.Parameters.AddWithValue("@description", survey.Description);
                        if (survey.PhotoUrl == null)
                        {
                            commandSurvey.Parameters.AddWithValue("@image_url", DBNull.Value);
                        }
                        else
                        {
                            commandSurvey.Parameters.AddWithValue("@image_url", survey.PhotoUrl);
                        }
                        commandSurvey.Parameters.AddWithValue("@creation_date", survey.CreationData);
                        commandSurvey.Parameters.AddWithValue("@end_date", survey.EndData);

                        var surveyId = (int)commandSurvey.ExecuteScalar();

                        foreach (var question in survey.Questions)
                        {

                            sql = @"
                            INSERT INTO Survey_Questions (survey_id, question_text) VALUES
                            (@survey_id, @question_text)
                            RETURNING question_id;
                            ";

                            using (var commandQuestion = new NpgsqlCommand(sql, connection))
                            {
                                commandQuestion.Parameters.AddWithValue("@survey_id", surveyId);
                                commandQuestion.Parameters.AddWithValue("@question_text", question.Text);

                                var qestionId = (int)commandQuestion.ExecuteScalar();

                                foreach (var answer in question.Answers)
                                {
                                    sql = @"
                                    INSERT INTO Survey_Responses (question_id, answer, score) VALUES
                                    (@question_id, @answer, @score);";

                                    using (var commandAnswer = new NpgsqlCommand(sql, connection))
                                    {
                                        commandAnswer.Parameters.AddWithValue("@question_id", qestionId);
                                        commandAnswer.Parameters.AddWithValue("@answer", answer.Text);
                                        commandAnswer.Parameters.AddWithValue("@score", answer.Score);
                                        commandAnswer.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Ошибка при вставке данных: " + ex.Message);
                }
            }
        }

        public static Survey GenerateAutoSurvey(DateTime endDate)
        {
            var survey = new Survey()
            {
                Id = -1,
                Title = "Опрос о удовлетворенности сотрудников",
                Description = "Анкета для оценки уровня удовлетворенности сотрудников условиями труда.",
                PhotoUrl = "http://localhost/base-images/1232.webp",
                CreationData = DateTime.Now,
                EndData = endDate,
            };
            {
                //1
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Как вы оцениваете свою общую мотивацию на работе по шкале от 1 до 5?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = -1,
                        Text = "5",
                        Score = 5,
                    });
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = -1,
                        Text = "4",
                        Score = 4,
                    });
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = -1,
                        Text = "3",
                        Score = 3,
                    });
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = -1,
                        Text = "2",
                        Score = 2,
                    });
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = -1,
                        Text = "1",
                        Score = 1,
                    });
                }

                //2
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Чувствуете ли вы, что ваши усилия на работе оцениваются по достоинству?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 1,
                        Text = "Да, полностью согласен(на)",
                        Score = 5 // Предположим, что 5 - это максимальная оценка
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 2,
                        Text = "Скорее да",
                        Score = 4
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 3,
                        Text = "Нейтрально",
                        Score = 3
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 4,
                        Text = "Скорее нет",
                        Score = 2
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 5,
                        Text = "Нет, совсем не согласен(на)",
                        Score = 1
                    });
                }

                //3
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Как вы оцениваете баланс между работой и личной жизнью в вашей компании?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 1,
                        Text = "Отлично",
                        Score = 5 // Максимальная оценка
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 2,
                        Text = "Хорошо",
                        Score = 4
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 3,
                        Text = "Удовлетворительно",
                        Score = 3
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 4,
                        Text = "Плохо",
                        Score = 2
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 5,
                        Text = "Очень плохо",
                        Score = 1
                    });
                }

                //4
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Насколько вы удовлетворены условиями труда в вашем офисе?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 1,
                        Text = "Очень удовлетворен(на)",
                        Score = 5 // Максимальная оценка
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 2,
                        Text = "Удовлетворен(на)",
                        Score = 4
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 3,
                        Text = "Нейтрально",
                        Score = 3
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 4,
                        Text = "Неудовлетворен(на)",
                        Score = 2
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 5,
                        Text = "Совсем не удовлетворен(на)",
                        Score = 1
                    });
                }

                //5
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Чувствуете ли вы себя комфортно на своем рабочем месте?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 1,
                        Text = "Да, полностью комфортно",
                        Score = 5 // Максимальная оценка
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 2,
                        Text = "Скорее комфортно",
                        Score = 4
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 3,
                        Text = "Нейтрально",
                        Score = 3
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 4,
                        Text = "Скорее некомфортно",
                        Score = 2
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 5,
                        Text = "Нет, совсем некомфортно",
                        Score = 1
                    });
                }

                //6
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Есть ли у вас все необходимые ресурсы для эффективного выполнения своих обязанностей?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 1,
                        Text = "Да, у меня есть все необходимые ресурсы",
                        Score = 5 // Максимальная оценка
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 2,
                        Text = "Скорее да, но есть небольшие недостатки",
                        Score = 4
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 3,
                        Text = "Нейтрально, иногда не хватает ресурсов",
                        Score = 3
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 4,
                        Text = "Скорее нет, часто не хватает ресурсов",
                        Score = 2
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 5,
                        Text = "Нет, у меня нет необходимых ресурсов",
                        Score = 1
                    });
                }

                //7
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Как бы вы оценили уровень стресса на вашей работе?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 1,
                        Text = "Очень высокий уровень стресса",
                        Score = 5 // Максимальная оценка стресса
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 2,
                        Text = "Высокий уровень стресса",
                        Score = 4
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 3,
                        Text = "Средний уровень стресса",
                        Score = 3
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 4,
                        Text = "Низкий уровень стресса",
                        Score = 2
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 5,
                        Text = "Очень низкий уровень стресса",
                        Score = 1 // Минимальная оценка стресса
                    });
                }

                //8
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Довольны ли вы системой премирования и бонусов в вашей компании?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 1,
                        Text = "Полностью доволен(на) системой премирования и бонусов",
                        Score = 5 // Максимальная оценка
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 2,
                        Text = "Скорее доволен(на)",
                        Score = 4
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 3,
                        Text = "Нейтрально, система приемлема",
                        Score = 3
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 4,
                        Text = "Скорее недоволен(на)",
                        Score = 2
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 5,
                        Text = "Совсем не доволен(на) системой премирования и бонусов",
                        Score = 1 // Минимальная оценка
                    });
                }

                //9
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Довольны ли вы возможностями для карьерного роста в вашей компании?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 1,
                        Text = "Полностью доволен(на) возможностями для карьерного роста",
                        Score = 5 // Максимальная оценка
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 2,
                        Text = "Скорее доволен(на)",
                        Score = 4
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 3,
                        Text = "Нейтрально, возможности приемлемы",
                        Score = 3
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 4,
                        Text = "Скорее недоволен(на)",
                        Score = 2
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 5,
                        Text = "Совсем не доволен(на) возможностями для карьерного роста",
                        Score = 1 // Минимальная оценка
                    });
                }

                //10
                survey.Questions.Add(new Survey.Question
                {
                    Id = -1,
                    Text = "Чувствуете ли вы, что ваша зарплата соответствует вашим усилиям и навыкам?"
                });

                {
                    var question = survey.Questions.Last();
                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 1,
                        Text = "Полностью согласен(на), зарплата соответствует моим усилиям и навыкам",
                        Score = 5 // Максимальная оценка
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 2,
                        Text = "Скорее согласен(на)",
                        Score = 4
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 3,
                        Text = "Нейтрально, зарплата приемлема",
                        Score = 3
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 4,
                        Text = "Скорее не согласен(на)",
                        Score = 2
                    });

                    question.Answers.Add(new Survey.Question.Answer
                    {
                        Id = 5,
                        Text = "Совсем не согласен(на), зарплата не соответствует моим усилиям и навыкам",
                        Score = 1 // Минимальная оценка
                    });
                }
            }
            return survey;
        }
    }
}
