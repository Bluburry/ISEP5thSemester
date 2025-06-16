export interface AvailabilitySlot {
    id: string;
    staffId: string;
    value: string;
  }

  export interface EditStaffDtoAdmin {
    LicenseNumber: string;
    firstName: string;
    lastName: string;
    fullName: string;
    email: string;
    phone: string;
    specialization: string;
    status: string;
    availabilitySlots: AvailabilitySlot[];
  }

  export interface StaffData {
    AvailabilitySlots: AvailabilitySlot[];
    Email: string;
    FirstName: string;
    Fullname: string;
    LastName: string;
    LicenseNumber: string;
    Phone: string;
    Specialization: string;
    Status: string;
  }
  