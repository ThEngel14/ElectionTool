CREATE PROCEDURE InsertErststimmen
AS

Declare     @id int,
			@electionId int,
			@wahlkreisId int,
            @personId int,
			@amount int

delete from Erststimme

Declare curP cursor For

  select Id, Election_Id, Wahlkreis_Id, Person_Id, Amount
  from ErststimmeAmount

OPEN curP 
Fetch Next From curP Into @id, @electionId, @wahlkreisId, @personId, @amount

While @@Fetch_Status = 0 Begin
	With numTable(num) as (select (num)
					From (Values(0), (1), (2), (3), (4), (5), (6), (7), (8), (9)) as MyTable(num)),
		idTable(id) as (select row_number() over ( order by a.num)
					from numTable a, numTable b, numTable c, numTable d, numTable e, numTable f)

	Insert into Erststimme(Election_Id, Wahlkreis_Id, Person_Id)
	select Election_Id, Wahlkreis_Id, Person_Id
	from ErststimmeAmount s, idTable t
	where s.Id = @id and t.id <= @amount

	Fetch Next From curP Into @id, @electionId, @wahlkreisId, @personId, @amount

End -- End of Fetch

Close curP
Deallocate curP
Go