# Highscore Server

A fast, lightweight server written in C# that lets you host leaderboards. Intended for use in Unity or other C# projects. Clients can add entries to the leaderboards or retrieve sections of the leaderboard.

## Status

**Work in Progress Alpha**: This project is in alpha and should only be used for testing purposes while it is still being developed. Contributions and feedback are welcome!

## Features

- Host multiple leaderboards
- Add new entries
- Retrieve sections of the leaderboard
- Lightweight and fast
- Easy to integrate with any software

## Getting Started

### Prerequisites

- [.NET 5.0 or later](https://dotnet.microsoft.com/download)

### Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/highscore-server.git
   
2. Navigate to the directory:
   ```sh
   cd HighscoreProject
   
3. Build the project
   ```sh
   dotnet build

### Usage

1. Start the server:
   ```sh
   cd HighscoreServer
   dotnet run
   
2. Server will start running on http://localhost:8080 as https and url support has not been implemented. Both the client and server have a secret int, found in HighscoreServer.cs and HighscoreClient.cs, make sure to change this for your project and have them match.
3. In your project, add the client directory to your .csproj file and include HighscoreClient in any code where you need to use it.

## Contributing
Contributions are welcome! Please follow these steps:

1. Fork the repository.

2. Create a new branch (git checkout -b feature-branch).

3. Make your changes.

4. Commit your changes (git commit -m 'Add new feature').

5. Push to the branch (git push origin feature-branch).

6. Open a pull request.

## Licence
This project is licensed under the MIT License. See the [LICENSE](LICENCE) file for details.

## Contact
If you have any questions or feedback, please open an issue.
