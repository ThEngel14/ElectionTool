USE ElectionDB
GO


alter function LoopInserterFunction(@Election_Id int, @Bundesland_Id int, @PartyId int, @Number int) 
				returns @result table(Election_Id int, Vorname varchar(30), Nachname varchar(30), HowCome varchar(30), BName varchar(30), PName varchar(30)) 
				as
begin 

declare @counter int = @Number;

while @counter >0
begin

	set @counter = @counter-1;

	insert into @result 
	
	select @Election_Id, pers.Firstname, pers.Lastname, 'Parteizugehoerigkeit',b.Name, p.Name
	from Bundesland b, Party p, CandidateList candidates, Person pers, PartyAffiliation partyAff
	where b.Id=@Bundesland_Id
	and p.Id=@PartyId 
	and candidates.Bundesland_Id= @Bundesland_Id
	and candidates.Election_Id = @Election_Id
	and candidates.Person_Id = pers.Id
	and partyAff.Election_Id = @Election_Id
	and partyAff.Person_Id= pers.Id
	and candidates.Position
	and pers.Id not in (select wks.Person_Id 
						from Wahlkreissieger wks
						where wks.ElectionID = @Election_Id )
						

end
				

	return;
end	