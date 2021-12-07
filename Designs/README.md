# Glory 2 Him - Design Overview
This platform will consist of the following functional modules.

## Modules
- Registration
- Profile
- Posts
- Reactions
- Comments
- Tags
- Attachments
- Links

# Registration
The platform will allow anonymous access, but to be able to contribute, users will be required to:
- Register the for an user account
- Validate their email address (registered users will operate in read only mode till validation are completed)

# Profile
This area will allow users to manage their user account and will cover things like
- changing your personal info like Name, Surname, Email, Mobile etc.
- changing your username
- linking external accounts like Microsoft, Google, Facebook Twitter (or any of the other external OAuth authentication providers) as an alternate way of signing in.
- setting up Two-factor authentication using TOTP (Time-based One-time Password Algorithm). This can be used together with any compliant authenticator app, including:
  - Microsoft Authenticator App
  - Google Authenticator App
  - LastPass Authenticator App

# Posts
Posts is the heart and soul of this platform.  
- All posts will be moderated to ensure the content meets the rules and guidelines.
- Any user will be able to submit a post and:
  - suggest tag(s) to associate a post to a topic
  - attach content to their post like a photo of video 
  - add links to external content like a YouTube video or a url to an article
- Users will also have the ability to contribute to existing posts by:
  - reacting to posts via emoji icons showing LIKE, LOVE, WOW etc...
  - suggesting tag(s) to associate a post to a topic
  - sugesting links to external content like a YouTube video or an Url to an article
  - adding a comment to a post

# Reactions
- Registered users will be able to show how they feel about posts and comments by association their reaction
- The amount of reactions and the reaction type i.e. LIKE, LOVE, WOW will be visible on comments and posts
- The amount of reactions on posts may influence which posts are displayed 1st on focussed views of posts

# Comments
- Comments are not moderated for existing user and they will have the ability to express their views freely within the agreed usage policy.
- Comments made by new users will be moderated for x days after they have verified their account.
  -  This is to ensure that comments are controlled and protected from abuse from new accounts.
- User will be able to react to posts via emoji icons showing LIKE, LOVE, WOW etc...
- As comments are mostly unmoderated, users will have a to report comments that does not meet the usage policy
  - User that are reported may be flagged for permanent moderation on all contributions OR be placed permanently in read-only mode.

# Tags
Tags are a quick way to group posts by topic.  Tags are moderated in two ways.
- If a new tag is suggested on a post, then the tag and the post will be subject to approval
  - If a tag association are approved for new tags, then the new tag will automatically be approved.
  - If ttag association is not approved, then the moderator still have the option to approve the tag.
- If a previously suggested tag is suggested as a tag for a post, then only the tag association is subject to apporoval
- All approved tags will surface as auto complete suggestion for users as they type.
- Tags can be promoted to allow special visibility in things like tag clouds where focus should be on the promoted tags rather than the popularity of the tag.

# Attachments
- Users will be able to attach content like photos or videos to posts.
- Attachments will be moderated as it forms part of posts
- Administrators may have the ability to autogenerate attachemnts from posts. i.e. a quote or saying can be added to an image of their choice
  -  This functionality will be usefull for sharing schenarios i.e. for Twitter the quote or saying might be used for the tweet, but when the post is shared to Instagram, then the image attachment is used.

# Links
- Users will be able to suggest links to external content like articles or videos and depending on the link type there might be different rules for presentation i.e. videos could be embedded while links to articles might redirect the user to the external content.
- Links will be moderated as it forms part of posts
