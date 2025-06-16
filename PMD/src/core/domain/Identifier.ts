/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-var-requires */
const { v4: uuid, validate: uuidValidate } = require('uuid');

export class Identifier<T> {
  constructor(private value: T) {
    this.value = value;
  }

  equals (id?: Identifier<T>): boolean {
    if (id === null || id === undefined) {
      return false;
    }
    if (!(id instanceof this.constructor)) {
      return false;
    }
    return id.toValue() === this.value;
  }

  toString () {
    return String(this.value);
  }

  /**
   * Return raw value of identifier
   */

  toValue (): T {
    return this.value;
  }
}
