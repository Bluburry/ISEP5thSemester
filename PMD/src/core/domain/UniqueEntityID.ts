/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-var-requires */
const uuid = require('uuid').v4;
import { Identifier } from './Identifier';

export class UniqueEntityID extends Identifier<string | number> {
  constructor(id?: string | number) {
    if (id && typeof id === 'string' && (id.startsWith('4A8Z.'))) {
      super(id);
    } else if (id && typeof id === 'number' || id && typeof id === 'string') {
      super(id);
    } else {
      super(uuid());
    }
  }
}
