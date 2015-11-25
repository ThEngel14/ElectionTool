CREATE PROCEDURE InsertZweitstimmen
AS

Declare     @id int,
			@electionId int,
			@partyId int,
			@wahlkreisId int,
			@amount int

delete from Zweitstimme

Declare curP cursor For

  select Id, Election_Id, Party_Id, Wahlkreis_Id, Amount
  from ZweitstimmeAmount

OPEN curP 
Fetch Next From curP Into @id, @electionId, @partyId, @wahlkreisId, @amount

While @@Fetch_Status = 0 Begin
	With numTable(num) as (select (num)
					From (Values(0), (1), (2), (3), (4), (5), (6), (7), (8), (9)) as MyTable(num)),
		idTable(id) as (select row_number() over ( order by a.num)
					from numTable a, numTable b, numTable c, numTable d, numTable e)

	Insert into Zweitstimme(Election_Id, Party_Id, Wahlkreis_Id)
	select Election_Id, Party_Id, Wahlkreis_Id
	from ZweitstimmeAmount s, idTable t
	where s.Id = @id and t.id <= @amount

	Fetch Next From curP Into @id, @electionId, @partyId, @wahlkreisId, @amount

End -- End of Fetch

Close curP
Deallocate curP
Go