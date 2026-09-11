const registerForm = document.getElementById("registerForm");

if (registerForm) {
    registerForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const name = document.getElementById("name").value.trim();
        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;
        const message = document.getElementById("message");

        try {
            const response = await fetch("/api/auth/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    name: name,
                    email: email,
                    password: password
                })
            });

            const data = await response.json();

            if (response.ok) {
                message.textContent = data.message || "Registration successful.";

                setTimeout(function () {
                    window.location.href = "login.html";
                }, 1000);
            } else {
                message.textContent = data.message || "Registration failed.";
            }
        } catch (error) {
            console.error(error);
            message.textContent = "Unable to connect to the API.";
        }
    });
}

const loginForm = document.getElementById("loginForm");

if (loginForm) {
    loginForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;
        const message = document.getElementById("message");

        try {
            const response = await fetch("/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    email: email,
                    password: password
                })
            });

            const data = await response.json();

            if (response.ok) {
                localStorage.setItem("token", data.token);
                localStorage.setItem("userId", data.userId);
                localStorage.setItem("name", data.name);
                localStorage.setItem("email", data.email);

                message.textContent = "Login successful. Redirecting...";

                setTimeout(function () {
                    window.location.href = "profile.html";
                }, 500);
            } else {
                message.textContent = data.message || "Login failed.";
            }
        } catch (error) {
            console.error(error);
            message.textContent = "Unable to connect to the API.";
        }
    });
}

const profileDetails = document.getElementById("profileDetails");

if (profileDetails) {
    loadProfile();
}

async function loadProfile() {
    const token = localStorage.getItem("token");
    const message = document.getElementById("message");
    const profileDetails = document.getElementById("profileDetails");

    if (!token) {
        message.textContent = "Please login first.";
        return;
    }

    try {
        const response = await fetch("/api/Profile", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {
            if (response.status === 401) {
                localStorage.removeItem("token");
                localStorage.removeItem("userId");
                localStorage.removeItem("name");
                localStorage.removeItem("email");
                message.textContent = "Session expired. Please login again.";
                return;
            }

            throw new Error("Failed to load profile.");
        }

        const data = await response.json();

        message.textContent = "Profile loaded successfully.";

        profileDetails.innerHTML = `
            <div>
                <strong>Name</strong>
                <p>${data.name || "Not provided"}</p>
            </div>

            <div>
                <strong>Email</strong>
                <p>${data.email || "Not provided"}</p>
            </div>

            <div>
                <strong>Bio</strong>
                <p>${data.bio || "Not provided"}</p>
            </div>

            <div>
                <strong>Experience Level</strong>
                <p>${data.experienceLevel || "Not provided"}</p>
            </div>

            <div>
                <strong>Availability</strong>
                <p>${data.availability || "Not provided"}</p>
            </div>

            <div>
                <strong>Session Duration</strong>
                <p>${data.preferredSessionDuration || "Not provided"}</p>
            </div>
        `;
    } catch (error) {
        console.error(error);
        message.textContent = "Unable to load profile.";
    }
}

const editProfileButton = document.getElementById("editProfileButton");
const editProfileForm = document.getElementById("editProfileForm");

if (editProfileButton && editProfileForm) {
    editProfileButton.addEventListener("click", function () {
        editProfileForm.style.display = "block";

        document.getElementById("bio").value = "";
        document.getElementById("experienceLevel").value = "";
        document.getElementById("availability").value = "";
        document.getElementById("preferredSessionDuration").value = "";
    });
}

const saveProfileButton = document.getElementById("saveProfileButton");

if (saveProfileButton) {
    saveProfileButton.addEventListener("click", async function () {
        const token = localStorage.getItem("token");
        const message = document.getElementById("message");

        if (!token) {
            message.textContent = "Please login first.";
            return;
        }

        const bio = document.getElementById("bio").value.trim();
        const experienceLevel = document.getElementById("experienceLevel").value.trim();
        const availability = document.getElementById("availability").value.trim();
        const preferredSessionDuration =
            document.getElementById("preferredSessionDuration").value.trim();

        try {
            const response = await fetch("/api/Profile", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    bio: bio,
                    experienceLevel: experienceLevel,
                    availability: availability,
                    preferredSessionDuration: preferredSessionDuration
                        ? parseInt(preferredSessionDuration, 10)
                        : null
                })
            });

            const data = await response.json();

            if (response.ok) {
                message.textContent =
                    data.message || "Profile updated successfully.";

                editProfileForm.style.display = "none";
                await loadProfile();
            } else {
                message.textContent =
                    data.message || "Profile update failed.";
            }
        } catch (error) {
            console.error(error);
            message.textContent = "Unable to connect to the API.";
        }
    });
}

async function loadSkills() {
    const token = localStorage.getItem("token");
    const skillSelect = document.getElementById("skillSelect");

    if (!token || !skillSelect) {
        return;
    }

    try {
        const response = await fetch("/api/Skills", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {
            console.error("Failed to load skills.");
            return;
        }

        const skills = await response.json();

        skillSelect.innerHTML =
            '<option value="">Select a skill</option>';

        skills.forEach(function (skill) {
            const option = document.createElement("option");

            option.value = skill.skillId;
            option.textContent = skill.skillName;

            skillSelect.appendChild(option);
        });
    } catch (error) {
        console.error(error);
    }
}

async function loadMySkills() {
    const token = localStorage.getItem("token");
    const skillsList = document.getElementById("mySkillsList");

    if (!token || !skillsList) {
        return;
    }

    try {
        const response = await fetch("/api/UserSkills", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {
            console.error("Failed to load my skills.");
            return;
        }

        const skills = await response.json();

        skillsList.innerHTML = "";

        if (skills.length === 0) {
            skillsList.innerHTML = "<p>No skills added yet.</p>";
            return;
        }

        skills.forEach(function (skill) {
            const skillDiv = document.createElement("div");

            skillDiv.innerHTML = `
                <h3>${skill.skillName}</h3>

                <p>
                    ${skill.category || "No category"}
                </p>

                <p>
                    ${skill.skillType === "OFFER"
                    ? "I can teach"
                    : "I want to learn"}
                    &bull;
                    ${skill.skillLevel || "Level not provided"}
                </p>

                <button
                    type="button"
                    onclick="editSkill(
                        ${skill.userSkillId},
                        '${skill.skillType}',
                        '${skill.skillLevel || ""}'
                    )">
                    Edit
                </button>

                <button
                    type="button"
                    onclick="deleteSkill(${skill.userSkillId})">
                    Delete
                </button>
            `;

            skillsList.appendChild(skillDiv);
        });
    } catch (error) {
        console.error(error);
    }
}

async function addSkill() {
    const token = localStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    const skillId = document.getElementById("skillSelect").value;
    const skillType = document.getElementById("skillType").value;
    const skillLevel = document.getElementById("skillLevel").value;

    if (!skillId) {
        alert("Please select a skill.");
        return;
    }

    if (!skillLevel) {
        alert("Please select a skill level.");
        return;
    }

    try {
        const response = await fetch("/api/UserSkills", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({
                skillId: parseInt(skillId, 10),
                skillType: skillType,
                skillLevel: skillLevel
            })
        });

        const result = await response.json();

        if (!response.ok) {
            alert(result.message || "Failed to add skill.");
            return;
        }

        alert(result.message || "Skill added successfully.");

        await loadMySkills();

        document.getElementById("skillSelect").value = "";
        document.getElementById("skillLevel").value = "";
    } catch (error) {
        console.error(error);
        alert("Unable to connect to the API.");
    }
}

async function editSkill(
    userSkillId,
    currentSkillType,
    currentSkillLevel
) {
    const token = localStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    const skillType = prompt(
        "Enter skill type: OFFER or WANT",
        currentSkillType
    );

    if (skillType === null) {
        return;
    }

    const skillLevel = prompt(
        "Enter skill level: Beginner, Intermediate or Advanced",
        currentSkillLevel
    );

    if (skillLevel === null) {
        return;
    }

    try {
        const response = await fetch(
            `/api/UserSkills/${userSkillId}`,
            {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    skillType: skillType,
                    skillLevel: skillLevel
                })
            }
        );

        const result = await response.json();

        if (!response.ok) {
            alert(result.message || "Failed to update skill.");
            return;
        }

        alert(result.message || "Skill updated successfully.");

        await loadMySkills();
    } catch (error) {
        console.error(error);
        alert("Unable to connect to the API.");
    }
}

async function deleteSkill(userSkillId) {
    const token = localStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    const confirmed = confirm(
        "Are you sure you want to delete this skill?"
    );

    if (!confirmed) {
        return;
    }

    try {
        const response = await fetch(
            `/api/UserSkills/${userSkillId}`,
            {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );

        const result = await response.json();

        if (!response.ok) {
            alert(result.message || "Failed to delete skill.");
            return;
        }

        alert(result.message || "Skill deleted successfully.");

        await loadMySkills();
    } catch (error) {
        console.error(error);
        alert("Unable to connect to the API.");
    }
}

async function loadMatches() {
    const token = localStorage.getItem("token");
    const matchesMessage = document.getElementById("matchesMessage");
    const matchesList = document.getElementById("matchesList");

    if (!matchesMessage || !matchesList) {
        return;
    }

    if (!token) {
        matchesMessage.textContent =
            "Please login to view your matches.";
        return;
    }

    try {
        const response = await fetch("/api/Matches", {
            method: "GET",
            headers: {
                "Authorization": "Bearer " + token
            }
        });

        if (!response.ok) {
            matchesMessage.textContent =
                "Unable to load matches.";
            return;
        }

        const matches = await response.json();

        matchesList.innerHTML = "";

        if (matches.length === 0) {
            matchesMessage.textContent =
                "No skill matches found.";
            return;
        }

        matchesMessage.textContent =
            "Users who can help you learn:";

        matches.forEach(function (match) {
            const matchDiv = document.createElement("div");

            matchDiv.className = "match-card";

            matchDiv.innerHTML = `
                <h3>${match.name}</h3>

                <p>
                    <strong>Skill:</strong>
                    ${match.skillName}
                </p>

                <p>
                    <strong>Level:</strong>
                    ${match.skillLevel || "Not provided"}
                </p>

                <p>
                    <strong>Email:</strong>
                    ${match.email}
                </p>

                <button
                    type="button"
                    class="request-session-button"
                    onclick="requestSession(
                        ${match.userId},
                        ${match.skillId}
                    )">
                    Request Session
                </button>
            `;

            matchesList.appendChild(matchDiv);
        });
    } catch (error) {
        console.error(error);

        matchesMessage.textContent =
            "Unable to connect to the API.";
    }
}

async function requestSession(receiverUserId, skillId) {
    const token = localStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    try {
        const response = await fetch(
            "/api/SessionRequests",
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify({
                    receiverUserId: receiverUserId,
                    skillId: skillId
                })
            }
        );

        const result = await response.json();

        if (!response.ok) {
            alert(
                result.message ||
                "Failed to send session request."
            );
            return;
        }

        alert(
            result.message ||
            "Session request sent successfully."
        );

        await loadMatches();
    } catch (error) {
        console.error(error);
        alert("Unable to connect to the API.");
    }
}

async function loadSessionRequests() {
    const token = localStorage.getItem("token");
    const requestsMessage =
        document.getElementById("requestsMessage");
    const requestsList =
        document.getElementById("requestsList");

    if (!requestsMessage || !requestsList) {
        return;
    }

    if (!token) {
        requestsMessage.textContent =
            "Please login to view session requests.";
        return;
    }

    try {
        const response = await fetch(
            "/api/SessionRequests",
            {
                method: "GET",
                headers: {
                    "Authorization": "Bearer " + token
                }
            }
        );

        if (!response.ok) {
            requestsMessage.textContent =
                "Unable to load session requests.";
            return;
        }

        const requests = await response.json();

        requestsList.innerHTML = "";

        if (requests.length === 0) {
            requestsMessage.textContent =
                "No session requests found.";
            return;
        }

        requestsMessage.textContent =
            "Your received session requests:";

        requests.forEach(function (request) {
            const requestDiv = document.createElement("div");

            requestDiv.className = "request-card";

            requestDiv.innerHTML =
                "<h3>" +
                request.requesterName +
                "</h3>" +

                "<p><strong>Skill:</strong> " +
                request.skillName +
                "</p>" +

                "<p><strong>Email:</strong> " +
                request.requesterEmail +
                "</p>" +

                "<p><strong>Status:</strong> " +
                request.status +
                "</p>" +

                (
                    request.status === "Pending"
                        ? (
                            "<button type='button' " +
                            "onclick='updateSessionRequest(" +
                            request.sessionRequestId +
                            ", \"Accepted\")'>" +
                            "Accept" +
                            "</button> " +

                            "<button type='button' " +
                            "onclick='updateSessionRequest(" +
                            request.sessionRequestId +
                            ", \"Rejected\")'>" +
                            "Reject" +
                            "</button>"
                        )
                        : ""
                );

            requestsList.appendChild(requestDiv);
        });
    } catch (error) {
        console.error(error);

        requestsMessage.textContent =
            "Unable to connect to the API.";
    }
}

async function updateSessionRequest(
    sessionRequestId,
    status
) {
    const token = localStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    try {
        const response = await fetch(
            `/api/SessionRequests/${sessionRequestId}`,
            {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(status)
            }
        );

        const result = await response.json();

        if (!response.ok) {
            alert(
                result.message ||
                "Failed to update request."
            );
            return;
        }

        alert(
            result.message ||
            "Session request updated successfully."
        );

        await loadSessionRequests();
    } catch (error) {
        console.error(error);
        alert("Unable to connect to the API.");
    }
}

document.addEventListener(
    "DOMContentLoaded",
    function () {
        const skillSelect =
            document.getElementById("skillSelect");

        const mySkillsList =
            document.getElementById("mySkillsList");

        const addSkillButton =
            document.getElementById("addSkillButton");

        if (skillSelect) {
            loadSkills();
        }

        if (mySkillsList) {
            loadMySkills();
        }

        if (document.getElementById("matchesList")) {
            loadMatches();
        }

        if (document.getElementById("requestsList")) {
            loadSessionRequests();
        }

        if (addSkillButton) {
            addSkillButton.addEventListener(
                "click",
                addSkill
            );
        }
    }
);