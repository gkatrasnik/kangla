# Agent Instructions

## Line Endings

- Preserve existing line endings when editing files.
- Do not convert files from LF to CRLF or from CRLF to LF unless the user explicitly asks for line-ending normalization.
- When creating new text files, use LF line endings.
- Before finishing changes on Windows, check the diff for line-ending-only churn and fix it before reporting completion.
