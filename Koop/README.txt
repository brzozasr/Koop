Aby uruchomić projekt:
- utworzyć pustą bazę w PostgreSQL;
- zaktualizować 'connection string' w pliku 'appsettings.json';
- utworzyć tabele w bazie komendą: 'dotnet ef database update';
- załadować mockowe dane - komendy 'INSERT' znajdują się w pliku 'koop_mock_data.sql';

W pliku 'Controllers/AuthController.cs' znajdują się endpoint'y do zarządzania użytkownikami.
W pliku 'Controllers/TestController.cs' znajdują się endpointy do testowania weryfikacji uwierzytelniania.

Zaobserwowany problem:
- nie działa poprawnie autoryzacja poprzez role. Zwykła autoryzacja oraz autoryzacja poprzez nazwę użytkownika działa poprawnie.
- ustawienia 'Policy' znajdują się w pliku 'Extensions/AuthExtensions.cs';

Odtworzenie problemu:
1) zalogować się jako użytkownik 'Szymon Wiechniak' (ma przypisaną rolę 'Default'):
  a) endpoint: localhost:5000/api/auth/signin
  b) body: (JSON):
    {
      "Email": "szymon@wiechniak.com",
      "Password": "Qwerty&7"
    }
2) wykorzystać zwrócony w wyniku logowania token w zapytaniach do poniższych endpoint'ów:
  a) localhost:5000/test/noauth
     - w wyniku zapytania powinien zostać zwrócony JSON z komunikatem "It works";
  b) localhost:5000/test/auth
     - w wyniku zapytania powinien zostać zwrócony JSON z komunikatem "It works";
  c) localhost:5000/test/authusername
     - w wyniku zapytania powinien zostać zwrócony JSON z komunikatem "It works";
  d) localhost:5000/test/authrole
     - w wyniku zapytania zwracany jest kod błędu 403 (Forbidden);

