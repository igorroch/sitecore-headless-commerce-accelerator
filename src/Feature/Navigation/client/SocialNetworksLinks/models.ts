//    Copyright 2020 EPAM Systems, Inc.
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

import * as Jss from 'Foundation/ReactJss/client';

export interface SocialNetworkLinkDataSource extends Jss.BaseDataSourceItem {
    uri: Jss.GraphQLField<Jss.LinkField>;
}

export interface SocialNetworksLinksDataSource extends Jss.BaseDataSourceItem {
    sectionTitle: Jss.GraphQLField<Jss.TextField>;
    links: Jss.GraphQLListField<SocialNetworkLinkDataSource>;
}

export interface SocialLinksStateProps {
    isPageEditingMode: boolean;
}

export interface SocialLinksOwnProps extends Jss.GraphQLRendering<SocialNetworksLinksDataSource> {}

export interface SocialLinksProps extends SocialLinksOwnProps, SocialLinksStateProps {}

export interface SocialLinksState extends Jss.SafePureComponentState {}

export interface AppState extends Jss.SitecoreState<{
    pageEditing: boolean
}> {}
