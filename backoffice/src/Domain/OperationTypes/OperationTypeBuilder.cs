using System;
using System.Collections.Generic;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.Specializations;

namespace DDDSample1.Domain.OperationTypes
{
	public class OperationTypeBuilder
	{
		private OperationTypeName _name;
		private List<OperationPhase> _phases;
		private List<RequiredSpecialist> _specialists;
		private EstimatedDuration _estimatedDuration;
		private OperationTypeEndDate _endDate;
		private OperationTypeStartDate _startDate;
		private int? _version;
		private OperationType _operationType;

		public OperationTypeBuilder WithOperationTypeName(string name)
		{
			_name = new OperationTypeName(name);
			return this;
		}

		public OperationTypeBuilder WithVersion(string version)
		{
			if (version != null && version != "")
				_version = int.Parse(version);
			return this;
		}

		public OperationTypeBuilder CreateOperationType()
		{
			if (_name == null)
				throw new ArgumentException("Operation name is required");
			// ArgumentNullException.ThrowIfNull(_name, "Operation name is required");
			int version = _version ?? 1;
			_operationType = new OperationType(_name, version);

			return this;
		}

		public OperationTypeBuilder WithOperationPhases(List<string> phaseNames, List<string> phaseDuration)
		{
			if (_operationType == null)
				throw new ArgumentException("Operation Type needs to be created before creation operation phases.");
			if (phaseDuration.Count != phaseNames.Count)
				return this;

			foreach (string s in phaseNames)
			{
				if (!Enum.TryParse<PhaseName>(s, true, out _))
					return this;
			}

			_phases = [];

			for (int i = 0; i < phaseNames.Count; i++)
			{
				Enum.TryParse<PhaseName>(phaseNames[i], true, out PhaseName pn);
				/* PhaseDuration pd = new PhaseDuration(phaseDuration[i]);
				if (pd == null)
					return this; */
				// var test = new OperationPhase(pn, phaseDuration[i], _operationType);
				// _phases.Add(test);
				_phases.Add(new OperationPhase(pn, phaseDuration[i]));
			}

			return this;
		}

		public OperationTypeBuilder WithRequiredSpecialists(List<Specialization> specializations, List<string> num, List<string> phase)
		{
			ArgumentNullException.ThrowIfNull(_operationType, "Operation Type needs to be created before creating required specialists.");
			if (specializations.Count != num.Count || specializations.Count != phase.Count)
				return this;

			foreach (string s in phase)
			{
				if (!Enum.TryParse<PhaseName>(s, true, out _))
					return this;
			}

			_specialists = [];

			for (int i = 0; i < specializations.Count; i++)
			{
				Enum.TryParse<PhaseName>(phase[i], true, out PhaseName pn);
				_specialists.Add(new RequiredSpecialist(specializations[i], new SpecialistCount(num[i]), _operationType, pn));
			}

			return this;
		}

		public OperationTypeBuilder WithEstimatedDuration(string duration)
		{
			if (duration != null && duration != "")
				_estimatedDuration = new EstimatedDuration(int.Parse(duration));

			return this;
		}

		public OperationTypeBuilder WithOperationStartDate(string startDate)
		{
			_startDate = new OperationTypeStartDate(DateTime.Parse(startDate));

			return this;
		}

		public OperationTypeBuilder WithOperationEndDate(string endDate)
		{
			_endDate = new OperationTypeEndDate(DateTime.Parse(endDate));

			return this;
		}

		public OperationType Build()
		{
			if (_operationType == null)
				throw new ArgumentException("Operation Type needs to be created before build.");
			if (_estimatedDuration == null)
				throw new ArgumentException("Estimated duration is required"); // can remove this
			if (_phases == null)
				throw new ArgumentException("Operation phases are required");
			if (_specialists == null)
				throw new ArgumentException("Specialists are required");
			if (_phases.Count < 3)
				throw new ArgumentException("A minimum of three operation phases are required");
			_operationType.AddOperationPhases(_phases);
			if (_specialists.Count == 0)
				throw new ArgumentException("A list of required specializations needs to be provided.");
			_operationType.AddRequiredSpecialists(_specialists);
			int sum = 0;
			foreach (OperationPhase op in _phases)
			{
				sum += op.PhaseDuration;
			}
			if (_estimatedDuration != null)
			{
				if (_estimatedDuration.Duration < sum && _estimatedDuration.Duration > sum + 15)
					throw new ArgumentException("Discrepancy between estimated duration and sum of phases duration.");
				_operationType.AddEstimatedDuration(_estimatedDuration);
			}
			else
				_operationType.AddEstimatedDuration(new EstimatedDuration(sum));

			// OperationType ot = new(_name, _phases, _specialists);

			return _operationType;
		}
	}
}