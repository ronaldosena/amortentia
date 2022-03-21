interface User {
  _id: string;
  username: string;
  gender: string;
  dateOfBirth: Date;
  age: number;
  fullName: string;
  knownAs: string;
  school: string;
  house: string;
  introduction?: string;
  lookingFor?: string;
  interests?: string[];
  city: string;
  country: string;
  photoUrl: string;
  photos?: Photo[];
  created: Date;
  lastActive: Date;
}

interface Photo {
  url: string;
  description: string;
  dateAdded: Date;
  isMain: boolean;
  publicId: string;
}

export { User, Photo };
