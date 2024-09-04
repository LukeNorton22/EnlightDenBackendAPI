# EnlightDenBackendAPI

## Overview

EnlightDenBackendAPI is a backend API developed as a senior capstone project to support a comprehensive study tool tailored to students' study needs. This API leverages OpenAI's capabilities to generate dynamic study aids, such as tests, flashcards, mind maps, and summarized notes, while allowing users to manage study plans and sessions. The goal is to enhance the learning experience by providing AI-powered, personalized study resources.

## Key Features

- AI-Powered Study Tools:
  - Generate customized quizzes and tests based on user-provided content.
  - Create flashcard sets for effective memorization.
  - Develop mind maps to visualize study topics and their relationships.
  - Summarize notes to provide concise study material.
  - Suggest additional content to complement learning materials.

- Study Management:
  - Allows users to create and manage study plans tailored to their learning schedules.
  - Supports tracking and management of study sessions.

- User and Class Management:
  - Enables user registration, authentication, and profile management.
  - Facilitates the organization of study materials by class and subject.

## Technology Stack

- Backend Framework: .NET 8, ASP.NET Core
- Object-Relational Mapping (ORM): Entity Framework Core
- Database: PostgreSQL
- AI Integration: OpenAI API for generating study content
- API Documentation: Swagger / OpenAPI for interactive API documentation and testing

## Architecture Overview

- Domain-Driven Design: The project follows a domain-driven design approach, ensuring modularity and clear separation of concerns.
- Service Layer: Encapsulates core business logic, interfacing between controllers and repositories.
- Repository Pattern: Provides a clean abstraction layer for data access, facilitating easier testing and code maintenance.
- Integration with OpenAI: Connects with OpenAI's API to dynamically generate study materials and tools tailored to the user's content.

