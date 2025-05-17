# Portal Gas

## Description

Portal Gas is a comprehensive web application designed for gas operations management, built with ASP.NET Core 8.0 and modern web technologies. This project provides a robust foundation for managing various aspects of gas operations and related business functions.

## Key Technologies

- **ASP.NET Core 8.0**: Provides high-performance, cross-platform web application framework
- **Entity Framework Core 9.0**: Enables efficient database operations and migrations
- **SQL Server**: Primary database for storing application data
- **Identity Framework**: Handles user authentication and authorization
- **MailKit**: Manages email communications within the application
- **Swagger**: API documentation and testing interface

## Features

### Core Modules
1. **User Management**
   - Role-based access control
   - User profile management
   - LDAP integration for enterprise authentication

2. **Work Overtime Management**
   - Overtime request submission
   - Approval workflows
   - Overtime tracking and reporting

3. **Document Management**
   - Secure file storage
   - Version control
   - Document sharing and collaboration

4. **Gas Monitoring**
   - Real-time gas level monitoring
   - Alert system
   - Historical data tracking

5. **Safety Management**
   - Job Safety Analysis (JSA)
   - Risk assessment
   - Incident reporting
   - Daily check-up management

## Screenshots

![Portal Dashboard](wwwroot/portal0.jpg)
*Main Dashboard Interface*

![Gas Monitoring](wwwroot/portal1.jpg)
*Gas Monitoring Module*

![Safety Management](wwwroot/portal2.jpg)
*Safety Management Interface*

## Setup Guide

### Prerequisites

1. **Development Environment**
   - Visual Studio 2022 or later
   - .NET 8.0 SDK
   - SQL Server 2019 or later
   - Git

2. **Required Software**
   - Node.js (for frontend dependencies)
   - SQL Server Management Studio (recommended)

### Installation Steps

1. **Clone and Setup**
   ```bash
   git clone https://github.com/revanza-git/portal-gas.git
   cd portal-gas
   ```

2. **Database Setup**
   - Open SQL Server Management Studio
   - Create a new database named "PortalGas"
   - Navigate to the `Migration` folder
   - Execute the SQL scripts in the following order:
     1. `01_CreateTables.sql`
     2. `02_SeedData.sql`

3. **Configuration**
   - Copy `appsettings.Example.json` to `appsettings.json`
   - Update the following settings:
     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=YOUR_SERVER;Database=PortalGas;Trusted_Connection=True;"
       },
       "EmailSettings": {
         "SmtpServer": "YOUR_SMTP_SERVER",
         "Port": 587,
         "Username": "YOUR_EMAIL",
         "Password": "YOUR_PASSWORD"
       }
     }
     ```

4. **Build and Run**
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

5. **Access the Application**
   - Open your browser and navigate to `https://localhost:5001`
   - Default admin credentials:
     - Username: admin@portal.com
     - Password: Admin@123

## Important Notes

### Sensitive Data Handling
Before using this project in production:

1. **Configuration**
   - Copy `appsettings.Example.json` to `appsettings.json`
   - Replace all placeholder values with your actual configuration
   - Never commit `appsettings.json` to version control
   - Add `appsettings.json` to `.gitignore`

2. **Company Information**
   - Remove any company-specific branding
   - Update all company-specific URLs and endpoints
   - Replace company email addresses with your own
   - Update any company-specific terminology

3. **Security**
   - Change all default passwords
   - Update API keys and secrets
   - Configure proper database credentials
   - Set up appropriate email server settings
   - Configure proper Active Directory settings

4. **Database**
   - Create a new database with your own schema
   - Update connection strings
   - Remove any company-specific data

- Remove any NR trademarks before using this project in production
- Update all connection strings and sensitive information
- Configure proper security settings before deployment
- Regular database backups are recommended

## Support and Contact

For support or inquiries, please contact:
- Email: revanza.raytama@gmail.com
- LinkedIn: [linkedin.com/in/revanzaraytama](https://linkedin.com/in/revanzaraytama)

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## Project Status

This project is actively maintained. For bug reports or feature requests, please open an issue on the repository.
