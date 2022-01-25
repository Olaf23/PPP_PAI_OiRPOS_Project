
# Wizualizacja efektywności produkcji (OEE)

## Opis Projektu

Głównym celem projektu było stworzenie aplikacji wizualizującej bieżący wskaźnik efektywności produkcji czyli tzw. OEE (ang. Overall Equipment Effectiveness). Wskaźnik ten jest miarą, która w prosty sposób informuje o ogólnej produktywności zarówno całego procesu wytwarzania, jak i pojedynczych stacji roboczych. Daje on możliwość przystępnej analizy przyczyn nieefektywnej produkcji oraz wpływu podejmowanych decyzji na stan produkcji. Na wskaźnik OEE składają się trzy podstawowe wartości:

- Dostępność - jest to stosunek zaplanowanego czasu pracy bez awarii do całego zaplanowanego czasu pracy
- Wydajność - Jest to stosunek ilości wyprodukowanych sztuk do założonego celu
- Jakość - Jest stosunkiem poprawnie wyprodukowanych sztuk do wszystkich

Wartość wskaźnika OEE jest iloczynem tych trzech wartości

## Struktura systemu aplikacji
System składa się z następujących elementów:
- Wizualizacja efektywności produkcji
- Panel operatora maszyny
- Aplikacja Rest API
- Baza danych SQL

# Wizualizacja
Jest to aplikacja web napisana w [Vue.js](https://vuejs.org/). Jej głównym celem jest pokazywanie aktualnego stanu wskaźników OEE oraz godzinowego podsumowania produkcji. Wyświetlane elementy:
- Wskaźniki OEE, Dostępność, Wydajność oraz Jakość
- Informacje o wyświetlanej stacji roboczej
- Tablica z podsumowaniem godzinowym ilości wyprodukowanych sztuk oraz celu na daną godzinę
- Suma czasu awarii
- Obecny stan pracy maszyny
- Podsumowanie godzinowe stanu pracy na daną godzinę

Do uruchomienia wizualizacji niezbędny jest działający serwer [Node.js](https://nodejs.org/en/)

#### Uruchomienie, skompilowanie serwera
```
npm run serve
```

#### Uruchomienie, skompilowanie serwera w trybie produkcji
```
npm run build
```
# Panel Operatora
Jest to aplikacja desktopowa napisana w języku Python, przy pomocy framework'a QT py. Docelowym zadaniem apliakcji jest umożliwienie operatorowi maszyny wprowadzenie podstawowych danych o produkcji. Aplikacja może być wyświetlana na przykład na panelu dotykowym.
Aplikacja umożliwia operatorowi maszyny:
- Dodawanie sztuk produkcyjnych z etykietą OK oraz NOK
- Nadawanie unikalnego kodu DMC dla każdej sztuki
- Określanie stanu pracy maszyny
	- Praca
	- Przerwa
	- Awaria

# Aplikacja Rest API
Aplikacja pełniąca rolę pośrednika komunikacji wszystkich aplikacji klienckich między sobą oraz bazą danych. Jest to centralny punkt całego systemu, który zarządza dostępem do informacji. Każda aplikacja kliencka, aby otrzymać żądane dane musi najpierw przejść odpowiednią autoryzację i otrzymać token za pomocą którego będzie identyfikowalna dla aplikacji API. Każde żądanie bez tokenu jest odrzucane. 
Aplikacja jest napisana w technologii ASP .Net C# webAPI. Uruchomić aplikację można na kilka sposobów:
- Jako aplikację konsolową (nie polecane)
- Zainstalować jako usługę Windows
- Dodać jako instancję w IIS

Głównym sposobem komunikacji w systemie jest wysyłanie żądań przez aplikacje klienckie na odpowiednie ścieżki zwane endpointami. Dostępne endpointy:
- **api/user/LoginIntoMES** - Pozwala uzyskać autoryzację po podaniu poprawnego loginu i hasła użytkownika. Zwraca token niezbędny dla dalszej komunikacji
- **api/user/LogOutFromMES** - Wylogowuje użytkownika z systemu, niszcząc przy tym jego token autoryzacji
- **oee/visualization/CheckConnection** - Sprawdza dostępność i poprawność komunikacji z API
- **oee/visualization/GetProductionStructure** - Zwraca informacje o strukturze produkcji przypisanej do użytkownika wysyłającego zapytanie
- **oee/visualization/GetOeeResult** - Zwraca paczkę danych z wszelkimi informacjami potrzebnymi do wyświetlenia wizualizacji
	- OeeRatingsCollection - Wartości każdego wskaźnika dla danej maszyny
	- OeeWorkTimesCollection - Czasy pracy i przestojów dla każdej z maszyn i ich czasy wystąpienia
	- OeeHourSummariesCollection - Informacje o podsumowaniu godzinowym dla każdej z maszyn
- **oee/plcs7driver/PutPartResult** - Pozwala na dodanie informacji o nowej sztuce produkcyjnej
-  **oee/plcs7driver/PutDowntime** - Umożliwia dodanie informacji o nowym statusie pracy
