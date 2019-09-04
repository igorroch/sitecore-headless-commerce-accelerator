//    Copyright 2019 EPAM Systems, Inc.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

import { keyMirror } from './../../utils';

export const actionTypes = keyMirror(
  {
    FIELD_CHANGE: null,
    FIELD_REGISTER: null,
    FIELD_UNREGISTER: null,
    FORM_CHANGE: null,
    UPDATE_STATUS: null,
  },
  '@FORM'
);
