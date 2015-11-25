Alter View ElectionParticipation as

Select p.Election_Id, p.Wahlkreis_Id, 1.0*p.Amount / a.Amount as Participation
from ParticipationAmount p join AllowedToElectAmount a on 
			p.Election_Id = a.Election_Id and p.Wahlkreis_Id = a.Wahlkreis_Id