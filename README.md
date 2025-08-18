# VakifBank - Digital Banking Application

A comprehensive digital banking application built with WPF and C# .NET 8.0, featuring modern banking services and real-time currency operations.

## Features

### Customer Operations
- User registration and authentication
- Account management (current and time deposit accounts)
- Money transfer between accounts
- Transaction history tracking

### Banking Services
- Credit card management with limit controls
- Debt payment functionality
- Risk scoring based on monthly income
- IBAN generation for accounts

### Smart Features
- Real-time currency exchange rates via TCMB API
- Currency buy/sell operations
- Time deposit account creation and management
- Automatic interest calculations

## Technologies Used

- **Frontend**: WPF (Windows Presentation Foundation)
- **Backend**: C# .NET 8.0
- **Database**: MySQL with Entity Framework 6
- **Testing**: MSTest for unit testing
- **Architecture**: MVVM pattern
- **API Integration**: TCMB (Turkish Central Bank) for exchange rates

## Project Structure

The application follows a modular architecture with separate components for:
- Customer management
- Account operations
- Credit card services
- Currency trading
- Transaction processing

## Getting Started

1. Clone the repository
2. Set up MySQL database
3. Configure connection strings
4. Build and run the application

## Testing

The project includes comprehensive unit tests covering:
- Model validations
- Business logic
- Database operations
- API integrations
