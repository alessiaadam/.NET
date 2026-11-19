# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
| R7 | problema era in services/BookingService.cs la linia 28 | prin skill ul generat | folosea <= in loc de <, deci intervalele nu erau tratate ca fiind semi-deschise |
|R5|lipsea din cod, nu era implementat faptul ca o rezervare nu poate incepe in trecut, lipsea din Services/bokingService.cs|am descoperit tot prin skill ul generat| acum Booking.Create întoarce StartInPast daca condtitia start< nowUtc vine in handler|
|R6|si asta lipsea din cod, nu era implementat faptul ca duarata maxima e de 4 ore|skill ul| am pus conditia Period.Duration>4 in Services/BookingService.cs si acum da 400 adica e TooLong |
|R8|rezervarile anulate blocau sala si dubla anulare dadea 204, a fost gasita in Services/BookingServices.cs la liniile 26-59| a fost gasita prin verificarile mentionate in skill|acum query ul filtreaza Status==Active si Booking.Cancel() intoarce 409|
