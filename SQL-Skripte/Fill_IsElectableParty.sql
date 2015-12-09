insert into IsElectableParty (Election_Id, Party_Id, Bundesland_Id)
select e.Id, p.Id, b.Id
from Election e, Bundesland b, Party p
where e.Id = 2 and p.Name != 'CSU' and p.Name != 'Parteilos'