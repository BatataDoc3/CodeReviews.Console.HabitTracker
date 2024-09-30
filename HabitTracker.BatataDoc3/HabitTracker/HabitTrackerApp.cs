﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HabitTracker.BatataDoc3.db;
using Microsoft.Data.Sqlite;

namespace HabitTracker.BatataDoc3.HabitTracker
{
    
    internal class HabitTrackerApp
    {
        private List<string> habits = new List<string>(); 
        private CRUD crud;

        public HabitTrackerApp(CRUD crud) {
            this.crud = crud;

            string line;
            StreamReader sr = new StreamReader(@"HabitTracker\habits.txt");
            line = sr.ReadLine();
            while (line != null) 
            { 
                habits.Add(line);
                line = sr.ReadLine();
            }
        }

        public void MainMenu()
        {
            while (true)
            {
                String start = "0) Exit\n1) View All Records\n2) Insert Record\n3) Delete Record\n4) Update Record";
                Console.WriteLine("=================\nMAIN MENU\n=================");
                Console.WriteLine("Please choose an option");
                Console.WriteLine(start);
                Console.WriteLine("---------------------------------------------------------");
                string input = Console.ReadLine();
                bool success = int.TryParse(input, out int option);
                if (!success )
                {
                    Console.WriteLine("Please choose a valid option\n\n");
                    continue;
                }

                switch(option)
                {
                    case 0:
                        return;
                    case 1:
                        string records = "=================\nVIEW ALL RECORDS\n=================\n";
                        Console.WriteLine(start);
                        ViewAllRecords();
                        break;
                    case 2:
                        InsertRecord();
                        break; 
                    case 3:
                        DeleteRecord();
                        break;
                    case 4:
                        UpdateRecord();
                        break;
                    default:
                        Console.WriteLine("Please choose a valid option\n\n");
                        continue;
                }
            }
        }

        private void ViewAllRecords()
        {
            
            List<Habit> habits = crud.GetAllRecords();
            if (habits.Count == 0)
            {
                Console.WriteLine("There is no data in the database");
                MainMenu();
            }
            PrintResults(habits);
        }


        private void UpdateRecord()
        {
            string start = "=================\nUPDATE RECORD\n=================\n";
            Console.WriteLine(start);
            while (true) 
            { 
                Console.WriteLine("Please insert the ID you want to update (press 's' to see all records and 'b' to go back)");
                Console.WriteLine("---------------------------------------------------------");
                string input = Console.ReadLine();
                bool isInt = int.TryParse(input, out int option);
                if (input.Equals("s")) ViewAllRecords();
                if (input.Equals("b")) return;
                else if (!isInt)
                {
                    Console.WriteLine("Please insert a valid value");
                    continue;
                }
                else if (!crud.CheckIfTheIdExists(option))
                {
                    Console.WriteLine("Id not present in the database! Please insert a valid value");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    AlterInfo(option);
                    return;
                }
            }
        }

        private void AlterInfo(int id)
        {
            while (true)
            {

                Console.WriteLine("\nWhat would you like to alter?\n1)Habit\n2)Date");
                Console.WriteLine("---------------------------------------------------------");
                string input = Console.ReadLine();
                bool isInt = int.TryParse(input, out int option);
                if (!isInt || (!input.Equals("1") && !input.Equals("2")))
                {
                    Console.WriteLine("Please insert a valid value");
                    continue;
                }
                else if (option == 1)
                {
                    ChangeHabit(id);
                    return;
                }
                else
                {
                    ChangeDate(id);
                    return;
                }
                
            }
        }


        private void ChangeHabit(int id)
        {
            while (true)
            {
                Console.WriteLine("\nWhat is the new habit you want to change to? (press 's' for a list of available habits)");
                Console.WriteLine("---------------------------------------------------------");
                string input = Console.ReadLine();
                if (input.Equals("s"))
                {
                    ShowHabits();
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                }
                else if (!habits.Contains(input))
                {
                    Console.WriteLine("Invalid Habit");
                    Console.ReadLine();
                }
                else
                {
                    bool update = crud.UpdateRecord(id, input);
                    if (update)
                    {
                        Console.WriteLine("\nRecord updated with success! Press any key to continue");
                        Console.ReadLine();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Error updating db. Press any key to continue");
                    }
                }
            }
        }

        private void ChangeDate(int id)
        {
            while (true)
            {
                Console.WriteLine("\nWhat is the new date you want to change to (YYYY-MM-DD)?");
                Console.WriteLine("---------------------------------------------------------");
                string input = Console.ReadLine();

                DateTime parsedDate;
                if (DateTime.TryParseExact(input, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    bool update = crud.UpdateRecord(id, parsedDate);
                    if (update)
                    {
                        Console.WriteLine("\nRecord updated with success! Press any key to continue");
                        Console.ReadLine();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Error updating db. Press any key to continue");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Format. Press any key to continue");
                    Console.ReadLine();
                }
            }
        }

        private void PrintResults(List<Habit> habits)
        {
            string head = String.Format("||{0,-3}||{1,-25}||{2,-10}||", "Id", "Habit", "Date");
            string top = new('=', head.Length);
            Console.WriteLine(top);
            Console.WriteLine(head);
            Console.WriteLine(top);
            foreach(Habit habit in habits)
            {
                Console.WriteLine("||{0,-3}||{1,-25}||{2,-10}||", habit.Id, habit.Name, habit.Date.ToShortDateString());

            }
            Console.WriteLine(top + "\n");
            Console.ReadLine();
        }



        private void DeleteRecord()
        {
            String start = "=================\nDELETE A RECORD\n=================\n";
            Console.WriteLine(start);
            while (true)
            {
                Console.WriteLine("Which id would you want to remove? (press 's' to see records and 'b' to go back)");
                Console.WriteLine("---------------------------------------------------------");
                string response = Console.ReadLine();
                if (response.Equals("s")) ViewAllRecords();
                else if (response.Equals("b")) return;
                else { 

                    bool parse = int.TryParse(response, out int id);
                    if (!parse)
                    {
                        Console.WriteLine("Please insert a valid id");
                        Console.ReadLine();
                        continue;
                    }
                    bool success = crud.DeleteRecord(id);
                    if (!success)
                    {
                        Console.WriteLine("The provided id does not exist");
                        Console.ReadLine();
                        continue;
                    }
                    Console.WriteLine("\nRecord deleted with success!\n");
                    Console.ReadLine();
                    return;
                }
            }
        }


        private void InsertRecord()
        {
            String start = "=================\nINSERT A NEW RECORD\n=================\n";
            Console.WriteLine(start);
            while (true) { 

                String habitsString = "\nWhat habit do you want to add? Write 's' for a list of available habits and 'b' to go back to the Main Menu\n---------------------------------------------------------";
                Console.WriteLine(habitsString);
                String input = Console.ReadLine();
                if (input.Equals("s"))
                {
                    ShowHabits();
                    Console.WriteLine("Press any key to go back");
                    Console.ReadLine();
                }
                else if (input.Equals("b"))
                {
                    return;
                }
                else if (habits.Contains(input))
                {
                    DateTime parsedTime = GetDate();
                    crud.InsertRecord(input, parsedTime);
                    Console.WriteLine("\nRecord added successfully!\nPress any key to continue");
                    Console.ReadLine();
                    return;
                    
                }
                else
                {
                    Console.WriteLine("Invalid Habit");
                    Console.ReadLine();
                }
            }
        }

        private DateTime GetDate()
        {
            while (true)
            { 
                Console.WriteLine("Input the Date of the Habit (YYYY-MM-DD)");
                Console.WriteLine("---------------------------------------------------------");
                string input = Console.ReadLine();

                DateTime parsedDate;
                if (DateTime.TryParseExact(input, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    return parsedDate;
                }
                else
                {
                    Console.WriteLine("invalid format");
                }
            }
        }


        private void ShowHabits()
        {
            Console.WriteLine("---------------------------------------------------------");
            foreach(string habit in habits)
            {
                Console.WriteLine(habit);
                
            }
            Console.WriteLine("---------------------------------------------------------");
        }
    }
}
