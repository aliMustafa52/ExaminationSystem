# Quiz Management System

A comprehensive online quiz platform built with ASP.NET Core and Entity Framework Core, allowing instructors to create and manage exams while students can take quizzes and view their results.

## Features

- **User Management**: Separate roles for instructors and students
- **Course System**: Instructors create courses and enroll students
- **Exam Creation**:
  - Two exam types: Quiz (multiple attempts) and Final (single attempt)
  - Manual or automatic question selection
  - Intelligent balancing of question difficulty levels
- **Question Bank**: Reusable questions with difficulty ratings (simple/medium/hard)
- **Results Tracking**: Immediate feedback for students, comprehensive reporting for instructors

## Technology Stack

- **Backend**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: SQL Server (or compatible relational database)
- **Authentication**: JWT-based with role authorization

## Key Technical Challenges Solved

1. Implemented complex business rules around exam attempts and question selection
2. Developed algorithm for balanced automatic exam generation
3. Designed efficient data model for tracking student results and exam history
4. Created robust API with proper authorization checks for all operations

## Installation

1. Clone the repository
2. Configure database connection in `appsettings.json`
3. Run database migrations: