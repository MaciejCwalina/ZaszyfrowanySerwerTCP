# Instrukcja Obsługi Projektu
## 1. Konfiguracja
Uruchom program. Jeśli folder ServerConfig nie istnieje, zostaniesz poproszony o wprowadzenie nazwy użytkownika.
Po wprowadzeniu nazwy użytkownika, program utworzy pliki konfiguracyjne w folderze ServerConfig. Następnie program zakończy działanie, ale pamiętaj, aby umieścić klucze w folderze Keys, folder Keys znajduje się w Dokumenty\ServerConfig.
## 2. Uruchomienie Serwera
Po konfiguracji uruchom program ponownie. Teraz program spróbuje połączyć się z serwerem na lokalnym adresie IP (127.0.0.1) i domyślnym porcie 9999. Możesz również podać adres IP i port jako argumenty wiersza poleceń.
## 3. Komunikacja
Po nawiązaniu połączenia wprowadzaj tekst w konsoli, a program automatycznie zaszyfruje i wyśle go do wszystkich podłączonych użytkowników.
Otrzymasz również wiadomości od innych użytkowników, które zostaną zdeszyfrowane i wyświetlone w konsoli.
## 4. Zakończenie
Jeśli użytkownik się rozłączy, zostaniesz powiadomiony o tym zdarzeniu, a klucz tego użytkownika zostanie usunięty.
Pamiętaj o umieszczeniu kluczy w odpowiednim folderze i sprawdź plik konfiguracyjny przed rozpoczęciem komunikacji. Teraz możesz korzystać z bezpiecznej, szyfrowanej komunikacji!
