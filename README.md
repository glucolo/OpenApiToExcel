# 📘 Excel documentation generator for Web APIs

This tool automates the creation of Web API documentation by converting its OpenAPI specification (JSON) into a structured Excel file. It is designed to help developers, technical writers, and RESTFul Web API users quickly visualize and understand available endpoints.

## 🎯 Purpose

The goal of this tool is to simplify the process of documenting APIs by extracting all relevant information from a OpenApi specification and organizing it into a human-readable Excel spreadsheet.

## 🔧 Features

- Parses OpenApi 2.0/3/3.1 JSON files  
- Extracts endpoint paths, HTTP methods, Request parameters, Response object
- Outputs a well-formatted Excel file with separate sheets for endpoints, Request parameters, Response objects
- Useful for technical documentation, API reviews, and onboarding new developers

## 📈 Use Cases

- Generate up-to-date API documentation for internal or external use  
- Share API structure with non-developers in a familiar Excel format  
- Facilitate API audits and compliance checks

---


## 📄 License
This project is released under a **custom license inspired by the GNU Affero General Public License v3 (AGPL-3.0)**, with the following conditions:

### ✅ Permitted Use
- Use, modification, and redistribution are allowed **only in non-commercial contexts**, such as:
  - Personal projects
  - Non-profit organizations
  - Charitable entities (ONLUS)
  - Public research institutions

### 🚫 Prohibited Use
- **Commercial use of the software, original or modified, is strictly prohibited** without **prior written authorization from the author**.
- This includes use by companies, for-profit entities, or in any revenue-generating context.

### 🌐 Network Use Clause (AGPL-like)
- If the software is used to provide services over a network (e.g., web applications, APIs), the complete source code **must be made available** to users of the service.

### ⚠️ Disclaimer
- The software is provided **"as is"**, without any warranty of any kind, express or implied.
- The author is **not liable** for any damages or issues arising from the use of this software.

### 📬 Commercial Requests
To obtain a commercial license or written authorization, please contact the author.

For more details, refer to the [LICENSE](./LICENSE) file included in this repository.


---


## 📝 Changelog

### v1.1.0
- Improved Excel formatting for better readability
- Added Microsoft.OpenApi v2.0.0 library for enhanced parsing capabilities
- Added support for additional Swagger/OpenAPI features
- Added response object extraction
- Bug fixes and performance improvements

### v1.0.0
- Initial release
- Parses Swagger/OpenAPI 2.0/3/3.1 JSON files
- Extracts endpoint paths, HTTP methods, request parameters
- Outputs a well-formatted Excel file with separate sheets for endpoints, request parameters



