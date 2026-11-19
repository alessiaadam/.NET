# AI-LOG

Write down, in order, what you asked the AI. It is not a punishment: a missing or false log lowers the level D grade.

| Time (approx.) | What you asked (briefly) | What you kept | What you changed or rejected |
|---|---|---|---|
| 2min 19s | un skill prin care i am zis sa mi implementeze partea A, sa foloseasca arhitectura vertical slices  | am pastrat in mare structura principala adica services, repositories, models si middleware | am sters endpoints si am adaugat Features cu Room si Bookings pt care am create si get, iar la booking in plus am cancel, toate sunt de forma request-response |
|2min 30s|un skill prin care i am zis sa mi implementeze partea B, sa am Infrastructure, RichDomain si CQRS|---|a sters Services, middleware si models. si a creat Domain si Infrastructure cu common si handlere, acum am constructori privati, settere private, endpointurile sunt thin|
|3min 2|un skill care m a ajutat sa implementez partea C|---|a adaugat pachetul Microsoft.EntityFrameworkCore.Sqlite 10.0.15 pt ca ultima versiune cere net11 si s ar fi stricat buil ul pe net 10; acum am interfete in domeniu dar fara referinte le EFCore; Add acum doar adauga entitatea iar salvarea e facuta de IUnitOfWork.SaveChangesAsync, a fost sters si Repositories|
