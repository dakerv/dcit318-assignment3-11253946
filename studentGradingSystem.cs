using System;
using System.Collections.Generic;
using System.IO;

namespace DCIT318_Q4
{
    // Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }
    }

    // Custom exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // StudentResultProcessor
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');

                    if (parts.Length < 3)
                        throw new MissingFieldException($"Missing field in line: {line}");

                    // Trim to remove extra spaces
                    var idPart = parts[0].Trim();
                    var namePart = parts[1].Trim();
                    var scorePart = parts[2].Trim();

                    if (!int.TryParse(idPart, out var id))
                        throw new InvalidScoreFormatException($"Invalid ID format in line: {line}");

                    if (!int.TryParse(scorePart, out var score))
                        throw new InvalidScoreFormatException($"Invalid score format in line: {line}");

                    students.Add(new Student(id, namePart, score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath, false))
            {
                foreach (var s in students)
                {
                    var grade = s.GetGrade();
                    writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {grade}");
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var processor = new StudentResultProcessor();

            string inputPath = "students_input.txt";
            string outputPath = "students_report.txt";

            try
            {
                var students = processor.ReadStudentsFromFile(inputPath);
                processor.WriteReportToFile(students, outputPath);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
